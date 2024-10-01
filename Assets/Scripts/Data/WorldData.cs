using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// WorldData 클래스
// 청크 상위의 정보를 저장하는 클래스
// 
// 자료형=========================================================================================
// worldName        >> 월드의 이름
// seed                 >> 지형 생성을 위한 시드값
// chunks              >> 월드에 있는 청크들의 정보
// modifiedChunks >> 
// 
// 메서드=========================================================================================
// RequestChunk(Vector2Int coord)
// 외부에서 청크를 호출하는 코드
// 월드 안에 이미 찾고 있는 청크가 있다면 그 청크 반환
// create가 false면 null반환?
// 청크가 생성되지 않았다면 LoadChunk() 사용하여 새로 생성
// 
// LoadChunk(Vector2Int coord)
// 청크가 처음 불러와지는 것이라면, 청크를 새로 생성함
// 
// IsVoxelInWorld(Vector3 pos)
// Vector3좌표가 맵 안에 있는지 반환
// 
// SetVoxel(Vector3 pos, ushort value)
// pos 위치에 id값 블록 세팅
// 
// GetVoxel(Vector3 pos)
// pos 위치에 블록의 정보 반환
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
