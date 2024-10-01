using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Noise
// ����� ����� �� �ְԲ� �ϴ� Ŭ����
// �ν��Ͻ�ȭ�� �ʿ� ���� static class = ������Ʈ�� ����� �� ���� �ٷ� �� �ҷ���
// 
// �޼���=========================================================================================
// get2DPerlin(Vector2 position, float offset, float scale)
// position = ��ǥ, offset = �õ尪 �ݿ�, scale = �������� �ϸ��� ���� �����Ͽ� ��ǥ���� �ش��ϴ� ����� ��ȯ
// 
// get3DPerlin(Vector3 position, float offset, float scale, float threshold)
// �⺻���� ��� ��Ŀ������ get2DPerlin�� ���� 
// threshold�� ���� ���Ͽ� ���������� true �ƴҽ� false
public static class Noise
{
    public static float get2DPerlin(Vector2 position, float offset, float scale)
    {
        position.x += (offset + VoxelData.seed + 0.1f);
        position.y += (offset + VoxelData.seed + 0.1f);

        return Mathf.PerlinNoise(position.x/ VoxelData.chunkWidth * scale, position.y / VoxelData.chunkWidth * scale);
    }

    public static bool get3DPerlin(Vector3 position, float offset, float scale, float threshold)
    {
        float x = (position.x + offset + VoxelData.seed + 0.1f) * scale, y = (position.y + offset + VoxelData.seed + 0.1f) * scale, z = (position.z + offset + VoxelData.seed + 0.1f) * scale;

        float xy = Mathf.PerlinNoise(x, y);
        float yz = Mathf.PerlinNoise(y, z);
        float xz = Mathf.PerlinNoise(x, z);

        float yx = Mathf.PerlinNoise(y, x);
        float zy = Mathf.PerlinNoise(z, y);
        float zx = Mathf.PerlinNoise(z, x);

        if ((xy + yz + xz + yx + zy + zx) / 6f > threshold) return true;
        else return false;
    }

}
