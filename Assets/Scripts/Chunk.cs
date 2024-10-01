using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Chunk 
// 청크 클래스
// 청크 오브젝트를 생성하고 배치하는 클래스인데, 메서드를 호출해야 해서 static을 붙이는듯 하다.
// 
// 자료형=========================================================================================
// ChunkCoord                                         >> 청크의 위치값 클래스
// Equals() = ChunkCoord값을 비교하여 이 ChunkCoord의 청크가 찾는 청크가 맞는지 확인
// 
// meshRenderer, meshFilter, meshCollider >> 새로 생성하는 청크 오브젝트에 해당 컴포넌트 추가하여 사용
// Chunk - MeshRenderer - MeshFilter 순으로 컴포넌트 정보가 이동함.(여기서 정의된 부분은 Chunk - MeshRenderer의 부분)
// meshCollider                                        >> 메쉬 정보를 이용한 충돌체 판정 
// 
// vertexIndex                                          >> AddVoxelDataToChunk()에 사용되는 꼭짓점 개수
// vertices / triangles                                >> AddVoxelDataToChunk()에서 꼭짓점과 삼각형 정보를 입력하고, CreateMesh()에서 완성된 리스트를 배열화하여 넘김
// uvs                                                      >> 텍스쳐를 입히기 위한 면 정보
// _isActive                                              >> 하단의 IsActive 실제 값
//// isVoxelMapPopulated                             >> 해당 좌표 블록정보가 생성되었는지
// transparentTriangles                              >> 투명한 블록 전용 머티리얼 구현을 위한 triangles
// materials                                              >> 일반 블럭과 투명한 블록 두가지 머티리얼을 담아두는 머티리얼 배열
//// modifications                                        >> 나무등 구조물 생성시 블록 배치를 위한 선입선출 큐
// position                                                >> chunkObject의 위치 정보(초기화 함수에서 선언됨)
// colors                                                   >> 청크의 그림자 효과를 위해 부여된 픽셀 색상값
// 
// IsActive                                                >> chunkObject가 게임에 보이는지에 대한 bool값 정의
//  - get = chunkObject.activeSelf
//  - set = chunkObject.SetActive(value)
// 
//// isEditable                                             >> 해당 청크가 수정 가능한지(복셀데이터 존재, 스레드 사용안함인지)
////  - get = !(!isVoxelMapPopulated)
// 
// chunkData                                             >> 청크의 정보
// 
// 
// 메서드=========================================================================================
// Chunk(ChunkCoord _coord)
// 클래스 생성자 (월드에서 자신을 인수로 전달하여 생성됨)
// 
//// Init()
//// chunkObject 게임 오브젝트를 생성 후 meshFilter, meshRenderer 전달
//// PopulateVoxelMap()을 실행하여 맵 정보 입력
//// chunkObject의 위치와 이름 초기화(ChunkCoord 좌표에 VoxelData.chunkWidth를 오프셋으로 전달)
//// 초기화가 완료된 chunkObject는 transform 컴포넌트 SetParent를 통해 World.instance오브젝트와 부모자식의 계층관계 생성
// 
// UpdateChunk()
// 청크를 구성하는 각각의 좌표들의 블록들을 AddVoxelDataToChunk로써 전달
// 
//// CheckVoxel(Vector3 position)
//// position = 블럭의 좌표
//// 해당되는 좌표에 블럭이 있는지 확인
//// 내 청크 안에 있는 블럭이 아니라면 청크 밖의 블럭인지 확인함
// 
// GetVoxelFormGlobalVector3(Vector3 pos)
// Vector3값 좌표에 해당하는 블럭 정보를 반환
// 
// UpdateMeshData(Vector3 position)
// 메쉬필터로 전해줄 복셀데이터 생성
// 면마다 꼭짓점 배열 4개 증가, 꼭짓점 배열을 활용한 삼각형의 꼭짓점 6개 사용
// position = 블럭의 좌표. 이거 없으면 모든 면이 0, 0, 0에 그려질거임
// CheckVoxel()을 통해서 중심이 되는 좌표에 블럭이 있는지, 있다면 주변 6개 면에 블럭이 붙어있는지(즉 인접한 면이라 구현할 필요가 없는지)
// 여기에서 CheckVoxel(중심 블럭 좌표)부분이 0이라 빈공간인데도 주변 6개 면을 체크하게끔 되어있는데 이거 컨티뉴로 생략 가능한지, 그럴 의미가 있는지 검토해봐야할듯
// 여기서 vertices와 AddTexture()의 uvs좌표가 1대1대응인 꼭짓점 정보라는것에 주의
// 
// CreateMesh()
// AddVoxelDataToChunk로 만들어진 데이터를 메쉬렌더러에 전달(각각의 리스트를 배열화한 후 전달)
// 
// AddTexture(int textureID)
// 텍스쳐 아틀라스에서 텍스쳐를 불러와 블럭의 각 면에 적용시킴
// 
// ClearMeshData()
// 플레이어가 블럭을 배치 혹은 파괴하였을 때 다시 메쉬를 생성하기 위한 초기화
// 
// EditVoxel(Vector3 pos, ushort newID)
// pos좌표에 newID블럭값을 적용(블럭배치 핵심코드)
// 블럭값을 적용하는것은 블럭을 추가배치할 수도 있지만 블럭이 제거된 것일수도 있으므로 해당 블럭에 인접하여 있는 청크를 모두 업데이트시킬 필요가 있음
// 따라서 블럭값 적용 후 UpdateSurroundingVoxels호출
// 
// UpdateSurroundingVoxels(int x, int y, int z)
// 해당 xyz좌표에 해당하는 블럭의 인접한 면이 다른 청크에 속해있는지 확인하고 주변 청크를 업데이트시킴
// 
//// CalculateLight()
//// 청크의 상단부터 빛에 가려진 그림자가 나오는지, 그림자는 얼마나 지는지 확인하는 메서드
// 
public class Chunk
{
    public ChunkCoord coord;
    GameObject chunkObject;
    MeshRenderer meshRenderer;
    MeshFilter meshFilter;

    int vertexIndex = 0;
    List<Vector3> vertices = new List<Vector3>();
    List<Vector2> uvs = new List<Vector2>();
    List<int> triangles = new List<int>(), transparentTriangles = new List<int>();
    Material[] materials = new Material[2];
    List<Color> colors = new List<Color>();
    List<Vector3> nomals = new List<Vector3>();

    public Vector3 position;

    //public Queue<VoxelMod> modifications = new Queue<VoxelMod>();

    private bool _isActive; //, isVoxelMapPopulated = false; 

    ChunkData chunkData;

    public Chunk(ChunkCoord _coord) 
    {
        coord = _coord;
        //IsActive = true;

        //if (generateOnLoad) Init();
    //}

    //public void Init()
    //{
        chunkObject = new GameObject();
        meshFilter = chunkObject.AddComponent<MeshFilter>();
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();

        materials[0] = World.instance.material;
        materials[1] = World.instance.transparentMaterial;
        meshRenderer.materials = materials;

        //meshRenderer.material = World.instance.material;
        chunkObject.transform.SetParent(World.instance.transform);

        chunkObject.transform.position = new Vector3(coord.x * VoxelData.chunkWidth, 0f, coord.z * VoxelData.chunkWidth);
        chunkObject.name = "Chunk " + coord.x + ", " + coord.z;
        position = chunkObject.transform.position;

        chunkData = World.instance.worldData.RequestChunk(new Vector2Int((int)position.x, (int)position.z), true);
        chunkData.chunk = this;

        //lock (World.instance.ChunkUpdateThreadLock) 
        World.instance.AddChunkToUpdate(this);

        if (World.instance.settings.enableAnimatedChunks) chunkObject.AddComponent<ChunkLoadAnimation>();

        //if (World.instance.enableThreading)
        //{
        //    Thread myThread = new Thread(new ThreadStart(PopulateVoxelMap));
        //    myThread.Start();
        //}
        //else 
            //PopulateVoxelMap();
    }

    //public void UpdateChunk()
    //{
    //    if (World.instance.enableThreading)
    //    {
    //        Thread myThread = new Thread(new ThreadStart(PopulateVoxelMap));
    //        myThread.Start();
    //    }
    //    else _updateChunk();
    //}

    public void UpdateChunk() 
    {
        //while(modifications.Count > 0)
        //{
        //    VoxelMod v = modifications.Dequeue();
        //    Vector3 pos = v.position -= position;
        //    chunkData.map[(int)pos.x, (int)pos.y, (int)pos.z].id = v.id;
        //}

        ClearMeshData();

        //CalculateLight();

        for (int x = 0; x < VoxelData.chunkWidth; x++)
            for (int y = 0; y < VoxelData.chunkHeight; y++)
                for (int z = 0; z < VoxelData.chunkWidth; z++)
                {
                    if (World.instance.blockTypes[chunkData.map[x, y, z].id].isSolid)
                        UpdateMeshData(new Vector3(x, y, z));
                }

        //lock (World.instance.chunksToDraw)
        //{
        World.instance.chunksToDraw.Enqueue(this);
        //}

        //threadLocked = false;
    }

    //void CalculateLight()
    //{
    //    Queue<Vector3Int> litVoxels = new Queue<Vector3Int>();

    //    for (int x = 0; x < VoxelData.chunkWidth; x++)
    //        for (int z = 0; z < VoxelData.chunkWidth; z++)
    //        {
    //            float lightray = 1f;

    //            for (int y = VoxelData.chunkHeight - 1; y > 0; y--)
    //            {
    //                VoxelState thisVoxel = chunkData.map[x, y, z];

    //                if (thisVoxel.id > 0 && World.instance.blockTypes[thisVoxel.id].transparency < lightray) lightray = World.instance.blockTypes[thisVoxel.id].transparency;

    //                thisVoxel.globalLightPercent = lightray;
    //                chunkData.map[x, y, z] = thisVoxel;

    //                if (lightray > VoxelData.lightFalloff) litVoxels.Enqueue(new Vector3Int(x, y, z));
    //            }
    //        }

    //    while(litVoxels.Count > 0)
    //    {
    //        Vector3Int v = litVoxels.Dequeue();

    //        for(int p = 0; p < 6; p++)
    //        {
    //            Vector3 currentVoxel = v + VoxelData.faceChecks[p];
    //            Vector3Int neighbor = new Vector3Int((int)currentVoxel.x, (int)currentVoxel.y, (int)currentVoxel.z);

    //            if (IsVoxelInChunk(neighbor.x, neighbor.y, neighbor.z))
    //            {
    //                if(chunkData.map[neighbor.x, neighbor.y, neighbor.z].globalLightPercent < chunkData.map[v.x, v.y, v.z].globalLightPercent - VoxelData.lightFalloff)
    //                {
    //                    chunkData.map[neighbor.x, neighbor.y, neighbor.z].globalLightPercent = chunkData.map[v.x, v.y, v.z].globalLightPercent - VoxelData.lightFalloff;

    //                    if (chunkData.map[neighbor.x, neighbor.y, neighbor.z].globalLightPercent > VoxelData.lightFalloff) litVoxels.Enqueue(neighbor);
    //                }
    //            }
    //        }
    //    }
    //}

    void ClearMeshData()
    {
        vertexIndex = 0;
        vertices.Clear();
        triangles.Clear();
        transparentTriangles.Clear();
        uvs.Clear();
        colors.Clear();
        nomals.Clear();
    }

    public bool IsActive
    {
        get { return _isActive; }
        set
        {
            _isActive = value;
            if(chunkObject != null) chunkObject.SetActive(value);
        }
    }

    //public Vector3 position
    //{
    //    get { return chunkObject.transform.position; }
    //}

    //public bool isEditable
    //{
    //    get
    //    {
    //        if (!isVoxelMapPopulated) return false; // || threadLocked
    //        else return true;
    //    }
    //}

    public void EditVoxel(Vector3 pos, ushort newID)
    {
        int x = Mathf.FloorToInt(pos.x), y = Mathf.FloorToInt(pos.y), z = Mathf.FloorToInt(pos.z);
        x -= Mathf.FloorToInt(chunkObject.transform.position.x);
        z -= Mathf.FloorToInt(chunkObject.transform.position.z);

        chunkData.ModifyVoxel(new Vector3Int(x, y, z), newID);
        //chunkData.map[x, y, z].id = newID;
        //World.instance.worldData.AddToModifiedChunkList(chunkData);

        //lock (World.instance.ChunkUpdateThreadLock)
        //{
        //World.instance.AddChunkToUpdate(this, true);
        UpdateSurroundingVoxels(x, y, z);
        //}

        //_updateChunk();
        //UpdateChunk();
    }

    void UpdateSurroundingVoxels(int x, int y, int z)
    {
        Vector3 thisVoxel = new Vector3(x, y, z);

        for (byte j = 0; j < 6; j++)
        {
            Vector3 currentVoxel = thisVoxel + VoxelData.faceChecks[j];

            if(!chunkData.IsVoxelInChunk((int)currentVoxel.x, (int)currentVoxel.y, (int)currentVoxel.z))
            {
                World.instance.AddChunkToUpdate(World.instance.GetChunkFromVector3(currentVoxel + position), true);
            }
        }
    }

    // VoxelState CheckVoxel(Vector3 pos)
    //{
    //    int x = Mathf.FloorToInt(pos.x), y = Mathf.FloorToInt(pos.y), z = Mathf.FloorToInt(pos.z);

    //    if (!chunkData.IsVoxelInChunk(x, y, z)) return World.instance.GetVoxelState(pos + position);
    //    //return World.instance.blockTypes[World.instance.GetVoxel(pos + position)].isSolid;
    //    return chunkData.map[x, y, z];
    //}

    public VoxelState GetVoxelFormGlobalVector3(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x), y = Mathf.FloorToInt(pos.y), z = Mathf.FloorToInt(pos.z);
        x -= Mathf.FloorToInt(position.x);
        z -= Mathf.FloorToInt(position.z);

        return chunkData.map[x, y, z];
    }

    void UpdateMeshData(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x), y = Mathf.FloorToInt(position.y), z = Mathf.FloorToInt(position.z);

        VoxelState voxel = chunkData.map[x, y, z];
        //ushort blockID = chunkData.map[x, y, z].id;
        //bool renderNeighborFaces = World.instance.blockTypes[blockID].renderNeighborFaces;

        for (byte j = 0; j < 6; j++)
        {
            VoxelState neighbor = chunkData.map[x, y, z].neighbours[j];

            if(neighbor != null && World.instance.blockTypes[neighbor.id].renderNeighborFaces)
            {
                vertices.Add(VoxelData.voxelVerts[VoxelData.vectorTris[j, 0]] + position);
                vertices.Add(VoxelData.voxelVerts[VoxelData.vectorTris[j, 1]] + position);
                vertices.Add(VoxelData.voxelVerts[VoxelData.vectorTris[j, 2]] + position);
                vertices.Add(VoxelData.voxelVerts[VoxelData.vectorTris[j, 3]] + position);

                for (int i = 0; i < 4; i++) nomals.Add(VoxelData.faceChecks[j]);

                AddTexture(voxel.properties.GetTextureID(j));

                float lightLevel = neighbor.lightAsFloat;
                //int ypos = (int)position.y + 1;
                //bool inShade = false;

                //while(ypos < VoxelData.chunkHeight)
                //{
                //    if(chunkData.map[(int)position.x, ypos, (int)position.z].id != 0)
                //    {
                //        inShade = true;
                //        break;
                //    }

                //    ypos++;
                //}

                //if (inShade) lightLevel = 0.5f;
                //else lightLevel = 0f;

                colors.Add(new Color(0, 0, 0, lightLevel));
                colors.Add(new Color(0, 0, 0, lightLevel));
                colors.Add(new Color(0, 0, 0, lightLevel));
                colors.Add(new Color(0, 0, 0, lightLevel));

                if (!neighbor.properties.renderNeighborFaces)
                {
                    triangles.Add(vertexIndex);
                    triangles.Add(vertexIndex + 1);
                    triangles.Add(vertexIndex + 2);
                    triangles.Add(vertexIndex + 2);
                    triangles.Add(vertexIndex + 1);
                    triangles.Add(vertexIndex + 3);
                }
                else
                {
                    transparentTriangles.Add(vertexIndex);
                    transparentTriangles.Add(vertexIndex + 1);
                    transparentTriangles.Add(vertexIndex + 2);
                    transparentTriangles.Add(vertexIndex + 2);
                    transparentTriangles.Add(vertexIndex + 1);
                    transparentTriangles.Add(vertexIndex + 3);
                }

                vertexIndex += 4;
            }
        }
    }

    public void CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();

        //mesh.triangles = triangles.ToArray();
        mesh.subMeshCount = 2;
        mesh.SetTriangles(triangles.ToArray(), 0);
        mesh.SetTriangles(transparentTriangles.ToArray(), 1);

        mesh.uv = uvs.ToArray();

        mesh.colors = colors.ToArray();

        mesh.normals = nomals.ToArray();
        //mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
    }

    void AddTexture(int textureID)
    {

        float y = textureID / VoxelData.textureAtlasSizeInBlocks * VoxelData.normalizedBlockTextureSize;
        float x = textureID % VoxelData.textureAtlasSizeInBlocks * VoxelData.normalizedBlockTextureSize;

        uvs.Add(new Vector2(x, y));
        uvs.Add(new Vector2(x, y + VoxelData.normalizedBlockTextureSize));
        uvs.Add(new Vector2(x + VoxelData.normalizedBlockTextureSize, y));
        uvs.Add(new Vector2(x + VoxelData.normalizedBlockTextureSize, y + VoxelData.normalizedBlockTextureSize));

    }
}

// ChunkCoord
// 청크의 위치에 대한 클래스
// 
// 자료형=========================================================================================
// x, z >> 청크의 좌표값
// 
// 메서드=========================================================================================
// Equals(ChunkCoord other)
// 해당 청크코드가 자신과 동일한지 확인
// 
public class ChunkCoord
{
    public int x, z;

    public ChunkCoord()
    {
        x = 0;
        z = 0;
    }

    public ChunkCoord(int _x, int _z)
    {
        x = _x;
        z = _z;
    }

    public ChunkCoord(Vector3 pos)
    {
        x = Mathf.FloorToInt(pos.x) / VoxelData.chunkWidth;
        z = Mathf.FloorToInt(pos.z) / VoxelData.chunkWidth;
    }

    public bool Equals(ChunkCoord other)
    {
        if (other == null) return false;
        else if (other.x == x && other.z == z) return true;
        else return false;
    }
}