using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// DebugScreen
// ����׽�ũ�� ������ ���� ������Ʈ
// 
// �ڷ���=========================================================================================
// world                           >> ���� ������Ʈ(����� ��ũ�� ������ ǥ���ϱ� ����)
// text                             >> ����׽�ũ�� ������Ʈ�� ǥ�õǴ� �ؽ�Ʈ ������Ʈ 
// frameRate                    >> 1�ʿ� ��� �������� �߻��Ͽ�����
// timer                           >> fps����� ���� Ÿ�̹�(1��)
// halfWorldSizeInVoxels   >> ���Ѹ� ��ǥ�� ���� ������
// halfWorldSizeInChunks >> ���Ѹ� ��ǥ�� ���� ������
// 
// �޼���=========================================================================================
// 

public class DebugScreen : MonoBehaviour
{
    World world;
    Text text;

    float frameRate, timer;
    int halfWorldSizeInVoxels, halfWorldSizeInChunks;

    void Start()
    {
        world = GameObject.Find("World").GetComponent<World>();
        text = GetComponent<Text>();
        gameObject.SetActive(false);

        halfWorldSizeInVoxels = VoxelData.worldSizeInVoxels / 2;
        halfWorldSizeInChunks = VoxelData.worldSizeInChunks / 2;
    }

    void Update()
    {
        string debugText = "Seintcat's making minecraft game with Unity\n";
        debugText += frameRate + " fps\n\n";
        debugText += "XYZ : " + (Mathf.FloorToInt(world.playerCamera.transform.position.x) - halfWorldSizeInVoxels) + " / " + Mathf.FloorToInt(world.playerCamera.transform.position.y) + " / " + (Mathf.FloorToInt(world.playerCamera.transform.position.z) - halfWorldSizeInVoxels) + "         ";
        debugText += "Chunk : " + (world.playerChunkCoord.x - halfWorldSizeInChunks) + " / " + (world.playerChunkCoord.z - halfWorldSizeInChunks) + "\n";

        text.text = debugText;

        if (timer > 1f)
        {
            frameRate = (int)(1f / Time.unscaledDeltaTime);
            timer = 0;
        }
        else timer += Time.deltaTime;
    }
}
