using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// WorldData Ŭ����
// ûũ ������ ������ �����ϴ� Ŭ����
// 
// �ڷ���=========================================================================================
// worldName        >> ������ �̸�
// seed                 >> ���� ������ ���� �õ尪
// chunks              >> ���忡 �ִ� ûũ���� ����
// modifiedChunks >> 
// 
// �޼���=========================================================================================
// RequestChunk(Vector2Int coord)
// �ܺο��� ûũ�� ȣ���ϴ� �ڵ�
// ���� �ȿ� �̹� ã�� �ִ� ûũ�� �ִٸ� �� ûũ ��ȯ
// create�� false�� null��ȯ?
// ûũ�� �������� �ʾҴٸ� LoadChunk() ����Ͽ� ���� ����
// 
// LoadChunk(Vector2Int coord)
// ûũ�� ó�� �ҷ������� ���̶��, ûũ�� ���� ������
// 
// IsVoxelInWorld(Vector3 pos)
// Vector3��ǥ�� �� �ȿ� �ִ��� ��ȯ
// 
// SetVoxel(Vector3 pos, ushort value)
// pos ��ġ�� id�� ��� ����
// 
// GetVoxel(Vector3 pos)
// pos ��ġ�� ����� ���� ��ȯ
// 
// 
[System.Serializable]
public class WorldData 
{
    public string worldName = "Prototype";
    public int seed;

    [System.NonSerialized]
    public Dictionary<Vector2Int, ChunkData> chunks = new Dictionary<Vector2Int, ChunkData>();

    [System.NonSerialized]
    public List<ChunkData> modifiedChunks = new List<ChunkData>();

    public void AddToModifiedChunkList(ChunkData chunk)
    {
        if (!modifiedChunks.Contains(chunk)) modifiedChunks.Add(chunk);
    }

    public WorldData(string _worldName, int _seed)
    {
        worldName = _worldName;
        seed = _seed;
    }
    public WorldData(WorldData wD)
    {
        worldName = wD.worldName;
        seed = wD.seed;
    }

    public ChunkData RequestChunk(Vector2Int coord, bool create)
    {
        ChunkData c;

        lock (World.instance.ChunkListThreadLock)
        {
            if (chunks.ContainsKey(coord))
                c = chunks[coord];
            else if (!create) c = null;
            else
            {
                LoadChunk(coord);
                c = chunks[coord];
            }
        }

        return c;
    }

    public void LoadChunk(Vector2Int coord)
    {
        if (chunks.ContainsKey(coord)) return;

        ChunkData chunk = SaveSystem.LoadChunk(worldName, coord);
        if(chunk != null)
        {
            chunks.Add(coord, chunk);
            return;
        }

        chunks.Add(coord, new ChunkData(coord));
        chunks[coord].Populate();
    }

    bool IsVoxelInWorld(Vector3 pos)
    {
        if (pos.x >= 0 && pos.x < VoxelData.worldSizeInVoxels && pos.y >= 0 && pos.y < VoxelData.chunkHeight && pos.z >= 0 && pos.z < VoxelData.worldSizeInVoxels) return true;
        else return false;
    }

    public void SetVoxel(Vector3 pos, ushort value)
    {
        if (!IsVoxelInWorld(pos)) return;

        int x = Mathf.FloorToInt(pos.x / VoxelData.chunkWidth), z = Mathf.FloorToInt(pos.z / VoxelData.chunkWidth);

        x *= VoxelData.chunkWidth;
        z *= VoxelData.chunkWidth;

        ChunkData chunk = RequestChunk(new Vector2Int(x, z), true);

        Vector3Int voxel = new Vector3Int((int)(pos.x - x), (int)pos.y, (int)(pos.z - z));

        chunk.ModifyVoxel(voxel, value);
    }

    public VoxelState GetVoxel(Vector3 pos)
    {
        if (!IsVoxelInWorld(pos)) return null;

        int x = Mathf.FloorToInt(pos.x / VoxelData.chunkWidth), z = Mathf.FloorToInt(pos.z / VoxelData.chunkWidth);

        x *= VoxelData.chunkWidth;
        z *= VoxelData.chunkWidth;

        ChunkData chunk = RequestChunk(new Vector2Int(x, z), false);

        if (chunk == null) return null;

        Vector3Int voxel = new Vector3Int((int)(pos.x - x), (int)pos.y, (int)(pos.z - z));

        return chunk.map[voxel.x, voxel.y, voxel.z];
    }
}
