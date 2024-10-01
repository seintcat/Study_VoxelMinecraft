using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ChunkData Ŭ����
// ûũ�� ������ �����ϴ� Ŭ����
// 
// �ڷ���=========================================================================================
// position  >> ûũ �ϳ��� ��ǥ(ûũ�ڵ�)
//  - x y = �ش� ������ ����Ͽ� Vector2Int ����
// 
// map       >> �ش� ��ǥ ���� ���� ����. 0���� �����, �׿ܼ��ڴ� ��ϵ�
// 
//// lightToPropogate
// 
// 
// 
// �޼���=========================================================================================
// Populate()
// ���� �����ϴ� ��ϵ� ������ �����Ͽ� ����
// 
//// AddForLightForPropogation(VoxelState voxel)
// 
// 
// 
// IsVoxelInChunk(int x, int y, int z)
// �� ûũ�� x, y, z��ǥ�� ���� �����ϴ°�?
// 
// VoxelFromV3Int(Vector3Int pos)
// 
// 
// 
// 
// 
// 
[System.Serializable]
public class ChunkData 
{
    int x, y;
    public Vector2Int position
    {
        get { return new Vector2Int(x, y); }
        set
        {
            x = value.x;
            y = value.y;
        }
    }

    //Queue<VoxelState> lightToPropogate = new Queue<VoxelState>();
    //public void AddForLightForPropogation(VoxelState voxel)
    //{
    //    lightToPropogate.Enqueue(voxel);
    //}

    public ChunkData(Vector2Int pos)
    {
        position = pos;
    }

    public ChunkData(int _x, int _y)
    {
        x = _x;
        y = _y;
    }

    [System.NonSerialized] public Chunk chunk;

    [HideInInspector]
    public VoxelState[,,] map = new VoxelState[VoxelData.chunkWidth, VoxelData.chunkHeight, VoxelData.chunkWidth];

    public void Populate()
    {
        for (byte y = 0; y < VoxelData.chunkHeight; y++)
            for (byte x = 0; x < VoxelData.chunkWidth; x++)
                for (byte z = 0; z < VoxelData.chunkWidth; z++)
                {
                    Vector3 voxelGlobalPos = new Vector3(x + position.x, y, z + position.y);

                    map[x, y, z] = new VoxelState(World.instance.GetVoxel(voxelGlobalPos), this, new Vector3Int(x, y, z));

                    for(int p = 0; p < 6; p++)
                    {
                        Vector3Int neighbourV3 = new Vector3Int(x, y, z) + VoxelData.faceChecks[p];

                        if (IsVoxelInChunk(neighbourV3)) map[x, y, z].neighbours[p] = VoxelFromV3Int(neighbourV3);
                        else map[x, y, z].neighbours[p] = World.instance.worldData.GetVoxel(voxelGlobalPos + VoxelData.faceChecks[p]);
                    }
                }

        Lighting.RecalculateNaturalLight(this);
        World.instance.worldData.AddToModifiedChunkList(this);
    }

    public void ModifyVoxel(Vector3Int pos, ushort _id)
    {
        if (map[pos.x, pos.y, pos.z].id == _id) return;

        VoxelState voxel = map[pos.x, pos.y, pos.z];
        BlockType newVoxel = World.instance.blockTypes[_id];

        byte oldOpacity = voxel.properties.opacity;

        voxel.id = _id;

        if(voxel.properties.opacity != oldOpacity && (pos.y == VoxelData.chunkHeight - 1 || map[pos.x, pos.y + 1, pos.z].light == 15))
        {
            Lighting.CastNaturalLight(this, pos.x, pos.z, pos.y + 1);
        }

        World.instance.worldData.AddToModifiedChunkList(this);

        if (chunk != null)
            World.instance.AddChunkToUpdate(chunk);
    }

    public bool IsVoxelInChunk(int x, int y, int z)
    {
        if (x < 0 || x >= VoxelData.chunkWidth || y < 0 || y >= VoxelData.chunkHeight || z < 0 || z >= VoxelData.chunkWidth) return false;
        else return true;
    }

    public bool IsVoxelInChunk(Vector3Int pos)
    {
        return IsVoxelInChunk(pos.x, pos.y, pos.z);
    }

    public VoxelState VoxelFromV3Int(Vector3Int pos)
    {
        return map[pos.x, pos.y, pos.z];
    }
}
