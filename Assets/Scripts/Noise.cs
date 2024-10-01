using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Noise
// 노이즈를 사용할 수 있게끔 하는 클래스
// 인스턴스화할 필요 없는 static class = 컴포넌트로 만드는 일 없이 바로 값 불러옴
// 
// 메서드=========================================================================================
// get2DPerlin(Vector2 position, float offset, float scale)
// position = 좌표, offset = 시드값 반영, scale = 노이즈의 완만함 정도 결정하여 좌표점에 해당하는 노이즈값 반환
// 
// get3DPerlin(Vector3 position, float offset, float scale, float threshold)
// 기본적인 계산 메커니즘은 get2DPerlin과 같음 
// threshold와 값을 비교하여 조건충족시 true 아닐시 false
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
