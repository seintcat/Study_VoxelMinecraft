using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Clouds
// 구름을 구현하는 컴포넌트
// 
// 자료형=========================================================================================
// cloudHeight     >> 구름이 생성되는 높이
// 
// cloudPattern    >> 구름 텍스쳐
// cloudData        >> 구름 텍스쳐를 기반으로 사각형 구름을 완성시키기 위한 부울 배열
// cloudTexWidth >> 구름 텍스쳐의 길이
// 
// cloudTileSize   >> 구름 타일 조각 하나당 사이즈
// offset              >> 각각 구름 타일 조각에 더해지는 위치 오프셋
// 
// clouds             >> 구름 타일 조각을 저장하는 딕셔너리 클래스
// 
// 
// 메서드=========================================================================================
// LoadCloudData()
// 텍스쳐에서 구름 정보를 생성
// 
// CreateClouds()
// 구름 타일 조각을 각각의 위치에 배치
// 
// CreateFastCloudMesh(int x, int z)
// x, z 좌표에 해당하는 구름 타일 메쉬 생성
// 
// CreateCloudTile(Mesh mesh, Vector3 position)
// 나뉘어진 구름 타일 조각을 생성
// 
// UpdateClouds()
// 각 구름 타일 조각의 위치를 다시 지정
// 
// RoundToCloud(float value)
// value값을 구름 타일 조각 크기 단위로 반올림하는 메서드
// 
// CloudTilePosFromV3(Vector3 pos)
// CloudTileCoordFromFloat()을 이용해 pos값에 있는 구름 타일 조각 코드 반환
// 
// CloudTileCoordFromFloat(float value)
// value를 통해 구름 타일 조각의 위치값 반환
// 
// CheckCloudData(Vector3Int point)
// point좌표에 구름이 있는지 확인한다(청크에서 수행하는 것과 동일 구조)
// 구름은 2D배열 정보로 구현하므로, 높이값인 y축은 사용하지 않는다.
// 
// 
public class Clouds : MonoBehaviour
{
    public int cloudHeight = 100, cloudDepth = 4;

    [SerializeField] private Texture2D cloudPattern = null;
    [SerializeField] private Material cloudMaterial = null;
    [SerializeField] private World world = null;

    bool[,] cloudData;

    int cloudTexWidth, cloudTileSize;

    Vector3Int offset;

    Dictionary<Vector2Int, GameObject> clouds = new Dictionary<Vector2Int, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        cloudTexWidth = cloudPattern.width;
        cloudTileSize = VoxelData.chunkWidth;
        offset = new Vector3Int(-(cloudTexWidth / 2), 0, -(cloudTexWidth / 2));

        transform.position = new Vector3(VoxelData.worldCenter, cloudHeight, VoxelData.worldCenter);

        LoadCloudData();
        CreateClouds();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LoadCloudData()
    {
        if (world.settings.clouds == CloudStyle.Off) return;

        cloudData = new bool[cloudTexWidth, cloudTexWidth];
        Color[] cloudTex = cloudPattern.GetPixels();

        for(int x = 0; x < cloudTexWidth; x++)
        {
            for (int y = 0; y < cloudTexWidth; y++)
            {
                cloudData[x, y] = (cloudTex[y * cloudTexWidth + x].a > 0.5);
            }
        }
    }

    private void CreateClouds()
    {
        if (world.settings.clouds == CloudStyle.Off) return;

        for (int x = 0; x < cloudTexWidth; x += cloudTileSize)
        {
            for (int y = 0; y < cloudTexWidth; y += cloudTileSize)
            {
                Mesh cloudmesh;
                if (world.settings.clouds == CloudStyle.Fast) cloudmesh = CreateFastCloudMesh(x, y);
                else cloudmesh = CreateFancyCloudMesh(x, y);

                Vector3 position = new Vector3(x, cloudHeight, y);
                position += transform.position - new Vector3(cloudTexWidth / 2f, 0f, cloudTexWidth / 2f);
                position.y = cloudHeight;
                clouds.Add(CloudTilePosFromV3(position), CreateCloudTile(cloudmesh, position));
                //CreateCloudTile(CreateCloudMesh(x, y), new Vector3(x, 0, y) + transform.position + offset);
            }
        }
    }

    public void UpdateClouds()
    {
        if (world.settings.clouds == CloudStyle.Off) return;

        for (int x = 0; x < cloudTexWidth; x += cloudTileSize)
        {
            for (int y = 0; y < cloudTexWidth; y += cloudTileSize)
            {
                Vector3 position = world.playerCamera.position + new Vector3(x, 0, y) + offset;
                position = new Vector3(RoundToCloud(position.x), cloudHeight, RoundToCloud(position.z));
                Vector2Int cloudPosition = CloudTilePosFromV3(position);

                clouds[cloudPosition].transform.position = position;
            }
        }
    }

    private int RoundToCloud(float value)
    {
        return Mathf.FloorToInt(value / cloudTileSize) * cloudTileSize;
    }

    private Mesh CreateFastCloudMesh(int x, int z)
    {
        List<Vector3> vertices = new List<Vector3>(), normals = new List<Vector3>();
        List<int> triangles = new List<int>();
        int vertCount = 0;

        for (int xIncrement = 0; xIncrement < cloudTileSize; xIncrement++)
        {
            for (int zIncrement = 0; zIncrement < cloudTileSize; zIncrement++)
            {
                int xVal = x + xIncrement, zVal = z + zIncrement;

                if (cloudData[xVal, zVal])
                {
                    vertices.Add(new Vector3(xIncrement, 0, zIncrement));
                    vertices.Add(new Vector3(xIncrement, 0, zIncrement + 1));
                    vertices.Add(new Vector3(xIncrement + 1, 0, zIncrement + 1));
                    vertices.Add(new Vector3(xIncrement + 1, 0, zIncrement));

                    for (int i = 0; i < 4; i++) normals.Add(Vector3.down);

                    triangles.Add(vertCount + 1);
                    triangles.Add(vertCount);
                    triangles.Add(vertCount + 2);
                    triangles.Add(vertCount + 2);
                    triangles.Add(vertCount);
                    triangles.Add(vertCount + 3);

                    vertCount += 4;
                }
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.normals = normals.ToArray();

        return mesh;
    }

    private Mesh CreateFancyCloudMesh(int x, int z)
    {
        List<Vector3> vertices = new List<Vector3>(), normals = new List<Vector3>();
        List<int> triangles = new List<int>();
        int vertexIndex = 0;

        for (int xIncrement = 0; xIncrement < cloudTileSize; xIncrement++)
        {
            for (int zIncrement = 0; zIncrement < cloudTileSize; zIncrement++)
            {
                int xVal = x + xIncrement, zVal = z + zIncrement;

                if (cloudData[xVal, zVal])
                {
                    for (int p = 0; p < 6; p++)
                    {
                        if (!CheckCloudData(new Vector3Int(xVal, 0, zVal) + VoxelData.faceChecks[p]))
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                Vector3 vert = new Vector3Int(xIncrement, 0, zIncrement);
                                vert += VoxelData.voxelVerts[VoxelData.vectorTris[p, i]];
                                vert.y *= cloudDepth;
                                vertices.Add(vert);
                            }

                            for (int i = 0; i < 4; i++) normals.Add(VoxelData.faceChecks[p]);

                            triangles.Add(vertexIndex);
                            triangles.Add(vertexIndex + 1);
                            triangles.Add(vertexIndex + 2);
                            triangles.Add(vertexIndex + 2);
                            triangles.Add(vertexIndex + 1);
                            triangles.Add(vertexIndex + 3);

                            vertexIndex += 4;
                        }
                    }
                }
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.normals = normals.ToArray();

        return mesh;
    }

    private bool CheckCloudData(Vector3Int point)
    {
        if (point.y != 0) return false;

        int x = point.x, z = point.z;

        if (point.x < 0) x = cloudTexWidth - 1;
        if (point.x > cloudTexWidth - 1) x = 0;
        if (point.z < 0) z = cloudTexWidth - 1;
        if (point.z > cloudTexWidth - 1) z = 0;

        return cloudData[x, z];
    }

    private GameObject CreateCloudTile(Mesh mesh, Vector3 position)
    {
        GameObject newCloudTile = new GameObject();
        newCloudTile.transform.position = position;
        newCloudTile.transform.parent = transform;
        newCloudTile.name = "Cloud " + position.x + ", " + position.z;
        MeshFilter mF = newCloudTile.AddComponent<MeshFilter>();
        MeshRenderer mR = newCloudTile.AddComponent<MeshRenderer>();

        mR.material = cloudMaterial;
        mF.mesh = mesh;

        return newCloudTile;
    }

    private Vector2Int CloudTilePosFromV3(Vector3 pos)
    {
        return new Vector2Int(CloudTileCoordFromFloat(pos.x), CloudTileCoordFromFloat(pos.z));
    }
    
    private int CloudTileCoordFromFloat(float value)
    {
        float a = value / (float)cloudTexWidth;
        a -= Mathf.FloorToInt(a);

        int b = Mathf.FloorToInt((float)cloudTexWidth * a);

        return b;
    }
}

// CloudStyle
// 구름 옵션값
// 
public enum CloudStyle
{
    Off,
    Fast,
    Fancy
}
