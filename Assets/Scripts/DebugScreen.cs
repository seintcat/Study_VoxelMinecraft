using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// DebugScreen
// 디버그스크린 구현을 위한 컴포넌트
// 
// 자료형=========================================================================================
// world                           >> 월드 컴포넌트(디버그 스크린 정보를 표시하기 위함)
// text                             >> 디버그스크린 오브젝트에 표시되는 텍스트 컴포넌트 
// frameRate                    >> 1초에 몇번 프레임이 발생하였는지
// timer                           >> fps계산을 위한 타이밍(1초)
// halfWorldSizeInVoxels   >> 유한맵 좌표를 위한 오프셋
// halfWorldSizeInChunks >> 유한맵 좌표를 위한 오프셋
// 
// 메서드=========================================================================================
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
