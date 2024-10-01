using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// VoxelState
// 블록의 종류와 밝기 등의 상태를 저장하는 클래스
// 
// 자료형=========================================================================================
// id                         >> 블록의 id값
//// globalLightPercent >> 쉐이더에 적용시킬 광원밝기의 퍼센트값 버전
// chunkData            >> 
// 
// properties             >> 
// 
// 
// 
// lightAsFloat           >> 
// 
// 
// 
// light                      >> 
// 
// 
// castLight
// 
// 메서드=========================================================================================
// PropogateLight()
// 
// 
// 
// 
// 
// 
// 
[System.Serializable]
public class VoxelState
{
    public ushort id;

    [System.NonSerialized]
    private byte _light;

    [System.NonSerialized]
    public ChunkData chunkData;

    [System.NonSerialized]
    public VoxelNeighbours neighbours;

    [System.NonSerialized]
    public Vector3Int position;

    public byte light
    {
        get { return _light; }
        set
        {
            if (value != _light)
            {
                //    _light = value;
                //    if (_light > 1)
                //        PropogateLight();
                byte oldLightValue = _light, oldCastValue = castLight;

                _light = value;

                if (_light < oldLightValue)
                {
                    List<int> neighboursToDarken = new List<int>();

                    for(int p = 0; p < 6; p++)
                    {
                        if(neighbours[p] != null)
                        {
                            if (neighbours[p].light <= oldCastValue) neighboursToDarken.Add(p);
                            else neighbours[p].PropogateLight();
                        }
                    }

                    foreach(int i in neighboursToDarken)
                    {
                        neighbours[i].light = 0;
                    }

                    if (chunkData.chunk != null) World.instance.AddChunkToUpdate(chunkData.chunk);
                }
                else if (_light > 1) PropogateLight();
            }
        }
    }

    //public float globalLightPercent;

    //public VoxelState()
    //{
    //    id = 0;
    //    globalLightPercent = 0f;
    //}

    public VoxelState(ushort _id, ChunkData _chunkData, Vector3Int _position)
    {
        id = _id;
        //globalLightPercent = 0f;
        chunkData = _chunkData;
        neighbours = new VoxelNeighbours(this);
        position = _position;
        light = 0;
    }

    public Vector3Int globalPosition
    {
        get
        {
            return new Vector3Int(position.x + chunkData.position.x, position.y, position.z + chunkData.position.y);
        }
    }

    public float lightAsFloat
    {
        get { return (float)light * VoxelData.unitOfLight; }
    }

    public byte castLight
    {
        get
        {
            int lightLevel = _light - properties.opacity - 1;
            if(lightLevel < 0) lightLevel = 0;

            return (byte)lightLevel;
        }
    }

    public void PropogateLight()
    {
        if (light < 2) return;

        for (int p = 0; p < 6; p++)
        {
            if (neighbours[p] != null)
            {
                if (neighbours[p].light < castLight)
                    neighbours[p].light = castLight;
            }

            if (chunkData.chunk != null) World.instance.AddChunkToUpdate(chunkData.chunk);
        }
    }

    public BlockType properties
    {
        get { return World.instance.blockTypes[id]; }
    }
}

// VoxelNeighbours
// 클래스
// 
// 자료형=========================================================================================
// parent
// Length
// 
// _neigbours
// VoxelNeighbours[]
// 
// 
// 
// 메서드=========================================================================================
// ReturnNeighbour(int index)
// 
// 
// 
// 

public class VoxelNeighbours
{
    public readonly VoxelState parent;
    
    public VoxelNeighbours(VoxelState _parent)
    {
        parent = _parent;
    }

    private VoxelState[] _neigbours = new VoxelState[6];
    public int Length
    {
        get
        {
            return _neigbours.Length;
        }
    }

    public VoxelState this[int index]
    {
        get
        {
            if(_neigbours[index] == null)
            {
                _neigbours[index] = World.instance.worldData.GetVoxel(parent.globalPosition + VoxelData.faceChecks[index]);
                ReturnNeighbour(index);
            }
            return _neigbours[index];
        }
        set
        {
            _neigbours[index] = value;
            ReturnNeighbour(index);
        }
    }

    void ReturnNeighbour(int index)
    {
        if (_neigbours[index] == null) return;

        if (_neigbours[index].neighbours[VoxelData.revFaceCheckIndex[index]] != parent)
            _neigbours[index].neighbours[VoxelData.revFaceCheckIndex[index]] = parent;
    }
}