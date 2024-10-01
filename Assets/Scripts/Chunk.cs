using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Chunk 
// ûũ Ŭ����
// ûũ ������Ʈ�� �����ϰ� ��ġ�ϴ� Ŭ�����ε�, �޼��带 ȣ���ؾ� �ؼ� static�� ���̴µ� �ϴ�.
// 
// �ڷ���=========================================================================================
// ChunkCoord                                         >> ûũ�� ��ġ�� Ŭ����
// Equals() = ChunkCoord���� ���Ͽ� �� ChunkCoord�� ûũ�� ã�� ûũ�� �´��� Ȯ��
// 
// meshRenderer, meshFilter, meshCollider >> ���� �����ϴ� ûũ ������Ʈ�� �ش� ������Ʈ �߰��Ͽ� ���
// Chunk - MeshRenderer - MeshFilter ������ ������Ʈ ������ �̵���.(���⼭ ���ǵ� �κ��� Chunk - MeshRenderer�� �κ�)
// meshCollider                                        >> �޽� ������ �̿��� �浹ü ���� 
// 
// vertexIndex                                          >> AddVoxelDataToChunk()�� ���Ǵ� ������ ����
// vertices / triangles                                >> AddVoxelDataToChunk()���� �������� �ﰢ�� ������ �Է��ϰ�, CreateMesh()���� �ϼ��� ����Ʈ�� �迭ȭ�Ͽ� �ѱ�
// uvs                                                      >> �ؽ��ĸ� ������ ���� �� ����
// _isActive                                              >> �ϴ��� IsActive ���� ��
//// isVoxelMapPopulated                             >> �ش� ��ǥ ��������� �����Ǿ�����
// transparentTriangles                              >> ������ ��� ���� ��Ƽ���� ������ ���� triangles
// materials                                              >> �Ϲ� ���� ������ ��� �ΰ��� ��Ƽ������ ��Ƶδ� ��Ƽ���� �迭
//// modifications                                        >> ������ ������ ������ ��� ��ġ�� ���� ���Լ��� ť
// position                                                >> chunkObject�� ��ġ ����(�ʱ�ȭ �Լ����� �����)
// colors                                                   >> ûũ�� �׸��� ȿ���� ���� �ο��� �ȼ� ����
// 
// IsActive                                                >> chunkObject�� ���ӿ� ���̴����� ���� bool�� ����
//  - get = chunkObject.activeSelf
//  - set = chunkObject.SetActive(value)
// 
//// isEditable                                             >> �ش� ûũ�� ���� ��������(���������� ����, ������ ����������)
////  - get = !(!isVoxelMapPopulated)
// 
// chunkData                                             >> ûũ�� ����
// 
// 
// �޼���=========================================================================================
// Chunk(ChunkCoord _coord)
// Ŭ���� ������ (���忡�� �ڽ��� �μ��� �����Ͽ� ������)
// 
//// Init()
//// chunkObject ���� ������Ʈ�� ���� �� meshFilter, meshRenderer ����
//// PopulateVoxelMap()�� �����Ͽ� �� ���� �Է�
//// chunkObject�� ��ġ�� �̸� �ʱ�ȭ(ChunkCoord ��ǥ�� VoxelData.chunkWidth�� ���������� ����)
//// �ʱ�ȭ�� �Ϸ�� chunkObject�� transform ������Ʈ SetParent�� ���� World.instance������Ʈ�� �θ��ڽ��� �������� ����
// 
// UpdateChunk()
// ûũ�� �����ϴ� ������ ��ǥ���� ��ϵ��� AddVoxelDataToChunk�ν� ����
// 
//// CheckVoxel(Vector3 position)
//// position = ���� ��ǥ
//// �ش�Ǵ� ��ǥ�� ���� �ִ��� Ȯ��
//// �� ûũ �ȿ� �ִ� ���� �ƴ϶�� ûũ ���� ������ Ȯ����
// 
// GetVoxelFormGlobalVector3(Vector3 pos)
// Vector3�� ��ǥ�� �ش��ϴ� �� ������ ��ȯ
// 
// UpdateMeshData(Vector3 position)
// �޽����ͷ� ������ ���������� ����
// �鸶�� ������ �迭 4�� ����, ������ �迭�� Ȱ���� �ﰢ���� ������ 6�� ���
// position = ���� ��ǥ. �̰� ������ ��� ���� 0, 0, 0�� �׷�������
// CheckVoxel()�� ���ؼ� �߽��� �Ǵ� ��ǥ�� ���� �ִ���, �ִٸ� �ֺ� 6�� �鿡 ���� �پ��ִ���(�� ������ ���̶� ������ �ʿ䰡 ������)
// ���⿡�� CheckVoxel(�߽� �� ��ǥ)�κ��� 0�̶� ������ε��� �ֺ� 6�� ���� üũ�ϰԲ� �Ǿ��ִµ� �̰� ��Ƽ���� ���� ��������, �׷� �ǹ̰� �ִ��� �����غ����ҵ�
// ���⼭ vertices�� AddTexture()�� uvs��ǥ�� 1��1������ ������ ������°Ϳ� ����
// 
// CreateMesh()
// AddVoxelDataToChunk�� ������� �����͸� �޽��������� ����(������ ����Ʈ�� �迭ȭ�� �� ����)
// 
// AddTexture(int textureID)
// �ؽ��� ��Ʋ�󽺿��� �ؽ��ĸ� �ҷ��� ���� �� �鿡 �����Ŵ
// 
// ClearMeshData()
// �÷��̾ ���� ��ġ Ȥ�� �ı��Ͽ��� �� �ٽ� �޽��� �����ϱ� ���� �ʱ�ȭ
// 
// EditVoxel(Vector3 pos, ushort newID)
// pos��ǥ�� newID������ ����(����ġ �ٽ��ڵ�)
// ������ �����ϴ°��� ���� �߰���ġ�� ���� ������ ���� ���ŵ� ���ϼ��� �����Ƿ� �ش� ���� �����Ͽ� �ִ� ûũ�� ��� ������Ʈ��ų �ʿ䰡 ����
// ���� ���� ���� �� UpdateSurroundingVoxelsȣ��
// 
// UpdateSurroundingVoxels(int x, int y, int z)
// �ش� xyz��ǥ�� �ش��ϴ� ���� ������ ���� �ٸ� ûũ�� �����ִ��� Ȯ���ϰ� �ֺ� ûũ�� ������Ʈ��Ŵ
// 
//// CalculateLight()
//// ûũ�� ��ܺ��� ���� ������ �׸��ڰ� ��������, �׸��ڴ� �󸶳� ������ Ȯ���ϴ� �޼���
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
// ûũ�� ��ġ�� ���� Ŭ����
// 
// �ڷ���=========================================================================================
// x, z >> ûũ�� ��ǥ��
// 
// �޼���=========================================================================================
// Equals(ChunkCoord other)
// �ش� ûũ�ڵ尡 �ڽŰ� �������� Ȯ��
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