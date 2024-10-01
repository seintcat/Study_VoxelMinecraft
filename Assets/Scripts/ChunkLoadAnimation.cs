using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ChunkLoadAnimation
// 청크가 자연스럽게 스폰되는 효과를 주기 위한 컴포넌트
// 
// 자료형=========================================================================================
// targetPos  >> 청크가 원래 위치했어야 하는 좌표
// speed       >> 청크 에니메이션의 스피드값
// waitTimer >> 청크가 생성되면서 애니메이션이 바로 나오면 메시가 안보이는경우가 생겨서 딜레이줌
// timer        >> 딜레이값만큼 기다리기 위한 계산용
// 
// 
// 메서드=========================================================================================
// 
// 
// 
public class ChunkLoadAnimation : MonoBehaviour
{
    Vector3 targetPos;
    float speed = 1f, waitTimer, timer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        waitTimer = Random.Range(0f, 3f);
        targetPos = transform.position;
        transform.position = new Vector3(transform.position.x, -VoxelData.chunkHeight, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        if(timer < waitTimer)
        {
            timer += Time.deltaTime;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * speed);
            if ((targetPos.y - transform.position.y) < 0.05f)
            {
                transform.position = targetPos;
                Destroy(this);
            }
        }
    }
}
