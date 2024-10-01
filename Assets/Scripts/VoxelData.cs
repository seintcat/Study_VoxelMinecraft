using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// VoxelData
// ���� �����ϱ� ���� ���� ������ �� ����
// �ν��Ͻ�ȭ�� �ʿ� ���� static class = ������Ʈ�� ����� �� ���� �ٷ� �� �ҷ���
// 
// �ڷ��� ���� ������ �ʿ䰡 ���� ���̹Ƿ� readonly
// ���� �ڷ����� int���ε� �� �� ���� ũ�� �ڷ��� ��� �����غ��߰���
// �ڷ���=========================================================================================
// chunkWidth                         >> ûũ ����(x, z)
// chunkHeight                        >> ûũ ����(y) 
// worldSizeInChunks               >> ���忡 �ʿ��� ûũ �� 
// textureAtlasSizeInBlocks       >> �ؽ�ó ��Ʋ���� ���μ��� �� ������
// normalizedBlockTextureSize  >> �ؽ�ó ��Ʋ�󽺸� (x, y)�� �������� ����ϱ� ���� ������(���� ���ʿ�)
// �ؽ�ó ��Ʋ�󽺴� ���簢���̾�� ����, ��Ʋ�� �ȿ� ��������Ʈ�� ��ǥ���� (0, 0 ���ϴ�) ~ (1, 1 ����)�����̱� ����
// 
// minLightLevel                      >> �ؽ����� �ּ� ��� ��ġ(���̴�)
// maxLightLevel                     >> �ؽ����� �ִ� ��� ��ġ(���̴�)
// 
// seed                                   >> ���� ������ ���� �õ� ����
// 
// worldCenter                        >> ���� �߽ɰ� ��ȯ
// - get = (worldSizeInChunks * chunkWidth) / 2
// 
// worldSizeInVoxels(readonly) >> ������� ��ũ�� ������ ���
//  - get = worldSizeInChunks * chunkWidth
// 
// unitOfLight                          >> 
// 
// revFaceCheckIndex              >> 
// 
// public static readonly Vector3[] voxelVerts = new Vector3[8]
// ������ü 8�� �������� ����
// 
// 
// public static readonly Vector3[] faceChecks = new Vector3[6]
// �� ���� �ٸ� ���ϰ� ����ִ��� Ȯ���ϱ� ���� 6�� ���� ����
// ������ back front top bottom left right
// 
// 
// public static readonly int[,] vectorTris = new int[6, 4]
// ���� �����ϱ� ���� �������� ����
// ������ ������ ����
// �簢 �� �ϳ� = �ΰ��� �ﰢ�� ���� = 6���� ������ ����(��, �������� �ݵ�� �ð�������� ��ġ�Ǿ�� �Ѵ�. ��, ������ �߿�)
// {0, 3, 1, 1, 3, 2 }, //back
// {5, 6, 4, 4, 6, 7 }, //front
// {3, 7, 2, 2, 7, 6 }, //top
// {1, 5, 0, 0, 5, 4 }, //bottom
// {4, 7, 0, 0, 7, 3 }, //left
// {1, 2, 5, 5, 2, 6 }  //right
// ������ �̷� �����̾�����, �� �迭�� ������ 0, 1, 2, 2, 1, 3���� ����ϴ� ������� ���߿� �ٲ�
// �޽��������� ���� ���� ����� �ϸ� �� 1�߰�, �� 1���, �� 2�߰� �� 2��� ������ ���� ������ ���� �ߺ��ؼ� �Է��ؾ���
// �� �ϳ��� ����Ҷ����� ����������Ʈ ���� �ΰ����� �߻��ϱ� ������ �̷� ����� ����ѵ�
// 
// 
// public static readonly Vector2[] voxelUvs = new Vector2[4]
// �ϳ��� �鿡 �ؽ��ĸ� ������ ���� ����
// �ﰢ���� ���� ���鶧 ���Ƿ�, �� ���� �������� ����س����� �ﰢ������ ���� ���鶧 �ڵ��ݿ�
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
