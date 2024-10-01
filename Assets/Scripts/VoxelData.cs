using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// VoxelData
// 맵을 구성하기 위한 복셀 정보가 들어가 있음
// 인스턴스화할 필요 없는 static class = 컴포넌트로 만드는 일 없이 바로 값 불러옴
// 
// 자료형 또한 변경할 필요가 없는 값이므로 readonly
// 많은 자료형이 int값인데 좀 더 작은 크기 자료형 사용 검토해봐야겠음
// 자료형=========================================================================================
// chunkWidth                         >> 청크 넓이(x, z)
// chunkHeight                        >> 청크 높이(y) 
// worldSizeInChunks               >> 월드에 필요한 청크 수 
// textureAtlasSizeInBlocks       >> 텍스처 아틀라스의 가로세로 블럭 사이즈
// normalizedBlockTextureSize  >> 텍스처 아틀라스를 (x, y)의 형식으로 사용하기 위한 단위값(수정 불필요)
// 텍스처 아틀라스는 정사각형이어야 좋고, 아틀라스 안에 스프라이트의 좌표값은 (0, 0 좌하단) ~ (1, 1 우상단)사이이기 때문
// 
// minLightLevel                      >> 텍스쳐의 최소 밝기 수치(쉐이더)
// maxLightLevel                     >> 텍스쳐의 최대 밝기 수치(쉐이더)
// 
// seed                                   >> 월드 생성을 위한 시드 정보
// 
// worldCenter                        >> 월드 중심값 반환
// - get = (worldSizeInChunks * chunkWidth) / 2
// 
// worldSizeInVoxels(readonly) >> 월드안의 블럭크기 사이즈 계산
//  - get = worldSizeInChunks * chunkWidth
// 
// unitOfLight                          >> 
// 
// revFaceCheckIndex              >> 
// 
// public static readonly Vector3[] voxelVerts = new Vector3[8]
// 정육면체 8개 꼭짓점의 정보
// 
// 
// public static readonly Vector3[] faceChecks = new Vector3[6]
// 한 블럭이 다른 블럭하고 닿아있는지 확인하기 위한 6개 면의 정보
// 순서는 back front top bottom left right
// 
// 
// public static readonly int[,] vectorTris = new int[6, 4]
// 면을 구성하기 위한 꼭짓점의 정보
// 순서는 다음과 같다
// 사각 면 하나 = 두개의 삼각면 정보 = 6개의 꼭짓점 정보(단, 꼭짓점은 반드시 시계방향으로 배치되어야 한다. 즉, 순서가 중요)
// {0, 3, 1, 1, 3, 2 }, //back
// {5, 6, 4, 4, 6, 7 }, //front
// {3, 7, 2, 2, 7, 6 }, //top
// {1, 5, 0, 0, 5, 4 }, //bottom
// {4, 7, 0, 0, 7, 3 }, //left
// {1, 2, 5, 5, 2, 6 }  //right
// 원래는 이런 느낌이었으나, 각 배열의 순서를 0, 1, 2, 2, 1, 3으로 사용하는 방식으로 도중에 바뀜
// 메쉬렌더러에 위와 같은 방식을 하면 접 1추가, 점 1사용, 점 2추가 점 2사용 식으로 가기 때문에 점을 중복해서 입력해야함
// 블럭 하나를 계산할때마다 꼭짓점리스트 정보 두개분이 발생하기 때문에 이런 방식을 사용한듯
// 
// 
// public static readonly Vector2[] voxelUvs = new Vector2[4]
// 하나의 면에 텍스쳐를 입히기 위한 정보
// 삼각형이 면을 만들때 들어가므로, 한 면의 꼭짓점을 등록해놓으면 삼각형으로 면을 만들때 자동반영
public static class VoxelData
{
    public static readonly byte chunkWidth = 16;
    public static readonly byte chunkHeight = 128;
    public static readonly ushort worldSizeInChunks = 100;

    public static float minLightLevel = 0.1f;
    public static float maxLightLevel = 0.9f;
    //public static float lightFalloff = 0.08f;

    public static float unitOfLight
    {
        get { return 1f / 16f; }
    }

    public static int seed;

    public static int worldCenter
    {
        get { return (worldSizeInChunks * chunkWidth) / 2; }
    }


    public static int worldSizeInVoxels
    {
        get { return worldSizeInChunks * chunkWidth; }
    }

    //public static readonly int ViewDistanceInChunks = 5;

    public static readonly int textureAtlasSizeInBlocks = 16;
    public static float normalizedBlockTextureSize
    {
        get { return  1f / (float)textureAtlasSizeInBlocks; }
    }

    public static readonly Vector3[] voxelVerts = new Vector3[8]
    {
        new Vector3(0.0f, 0.0f, 0.0f),
        new Vector3(1.0f, 0.0f, 0.0f),
        new Vector3(1.0f, 1.0f, 0.0f),
        new Vector3(0.0f, 1.0f, 0.0f),
        new Vector3(0.0f, 0.0f, 1.0f),
        new Vector3(1.0f, 0.0f, 1.0f),
        new Vector3(1.0f, 1.0f, 1.0f),
        new Vector3(0.0f, 1.0f, 1.0f)
    };

    public static readonly Vector3Int[] faceChecks = new Vector3Int[6]
    {
        //back front top bottom left right
        new Vector3Int(0, 0, -1), //back
        new Vector3Int(0, 0, 1), //front
        new Vector3Int(0, 1, 0), //top
        new Vector3Int(0, -1, 0), //bottom
        new Vector3Int(-1, 0, 0), //left
        new Vector3Int(1, 0, 0)  //right
    };

    public static readonly int[] revFaceCheckIndex = new int[6] { 1, 0, 3, 2, 5, 4 };

    public static readonly int[,] vectorTris = new int[6, 4]
    {
        {0, 3, 1, 2 }, //back
        {5, 6, 4, 7 }, //front
        {3, 7, 2, 6 }, //top
        {1, 5, 0, 4 }, //bottom
        {4, 7, 0, 3 }, //left
        {1, 2, 5, 6 }  //right

    };

    public static readonly Vector2[] voxelUvs = new Vector2[4]
    {
        new Vector2(0.0f, 0.0f), //0
        new Vector2(0.0f, 1.0f), //1
        new Vector2(1.0f, 0.0f), //2
        new Vector2(1.0f, 1.0f), //3
    };
}
