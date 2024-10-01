using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Structures
// 나무 등 구조물을 생성하는 클래스
// 인스턴스화할 필요 없는 static class = 컴포넌트로 만드는 일 없이 바로 값 불러옴
// 
// 메서드=========================================================================================
// MakeTree (Vector3 position, Queue<VoxelMod> queue, int minTrunkHeight, int maxTrunkHeight)
// 해당 포지션부터 minTrunkHeight ~ maxTrunkHeight까지의 나무기둥을 만듬
// 
// 
// 
// 
// 
// 
public static class Structure
{
    public static Queue<VoxelMod> GenerateMajorFlora(int index, Vector3 position, int minTrunkHeight, int maxTrunkHeight)
    {
        switch (index)
        {
            case 0:
                return MakeTree(position, minTrunkHeight, maxTrunkHeight);
            case 1:
                return MakeCati(position, minTrunkHeight, maxTrunkHeight);
        }

        return new Queue<VoxelMod>();
    }

    public static Queue<VoxelMod> MakeTree(Vector3 position, int minTrunkHeight, int maxTrunkHeight)
    {
        Queue<VoxelMod> queue = new Queue<VoxelMod>();

        int height = (int)(maxTrunkHeight * Noise.get2DPerlin(new Vector2(position.x, position.z), 250f, 3f));

        if (height < minTrunkHeight) height = minTrunkHeight;

        for (int i = 1; i < height; i++)
        {
            queue.Enqueue(new VoxelMod(new Vector3(position.x, position.y + i, position.z), 7));
        }

        int x, y, z;
        for (x = -3; x < 4; x++)
            for (y = 0; y < 7; y++)
                for (z = -3; z < 4; z++)
                {
                    queue.Enqueue(new VoxelMod(new Vector3(position.x + x, position.y + height + y, position.z + z), 8));
                }

        return queue;
    }

    public static Queue<VoxelMod> MakeCati(Vector3 position, int minTrunkHeight, int maxTrunkHeight)
    {
        Queue<VoxelMod> queue = new Queue<VoxelMod>();

        int height = (int)(maxTrunkHeight * Noise.get2DPerlin(new Vector2(position.x, position.z), 23456f, 2f));

        if (height < minTrunkHeight) height = minTrunkHeight;

        for (int i = 1; i <= height; i++)
        {
            queue.Enqueue(new VoxelMod(new Vector3(position.x, position.y + i, position.z), 9));
        }

        return queue;
    }
}
