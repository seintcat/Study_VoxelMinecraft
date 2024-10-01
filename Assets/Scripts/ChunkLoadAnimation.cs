using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ChunkLoadAnimation
// ûũ�� �ڿ������� �����Ǵ� ȿ���� �ֱ� ���� ������Ʈ
// 
// �ڷ���=========================================================================================
// targetPos  >> ûũ�� ���� ��ġ�߾�� �ϴ� ��ǥ
// speed       >> ûũ ���ϸ��̼��� ���ǵ尪
// waitTimer >> ûũ�� �����Ǹ鼭 �ִϸ��̼��� �ٷ� ������ �޽ð� �Ⱥ��̴°�찡 ���ܼ� ��������
// timer        >> �����̰���ŭ ��ٸ��� ���� ����
// 
// 
// �޼���=========================================================================================
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
