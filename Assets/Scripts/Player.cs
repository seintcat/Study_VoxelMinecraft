using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Player
// 움직이는 플레이어를 정의한 컴포넌트
// 
// 자료형=========================================================================================
// cam                                         >> 플레이어 시야(카메라)의 위치정보
// world                                       >> 플레이어의 충돌체 판정을 위한 월드 정보
// horizontal / vertical                   >> 플레이어 전후좌우 이동키 값
// mouseHrizontal / mouseVertical >> 마우스 X좌표, 마우스 Y좌표값
// velocity                                    >> 플레이어 키보드 입력에 따른 이동 좌표값
// walkSpeed                                >> 플레이어 이동속도
// gravity                                     >> 플레이어 중력
// sprintSpeed                              >> 플레이어 달리기 속도
// jumpForce                                >> 플레이어 점프력
// playerWidth                              >> 플레이어 두께
// verticalMomentum                     >> 현재 코드의 경우 중력가속도를 표현
// jumpRequest                             >> 플레이어의 점프키 입력 여분
// isGrounded                               >> 플레이어가 땅에 닿았는지 여부
// isSprinting                                >> 플레이어의 달리기키 입력 여부
// highlightBlock                           >> 제거되는 블럭의 위치
// placeBlock                                >> 생성되는 블럭의 위치
// checkIncrement                        >> 레이캐스트 시 블럭이 존재하는지 체크하는 거리 단위
// reach                                       >> 블럭을 배치할 수 있는 거리
// selectedBlockText                      >> 현재 손에 들고있는 블럭 이름 UI
// selectedBlockIndex                    >> 월드컴포넌트에 저장된 블럭 인덱스
// toolbar                                      >> 툴바
// 
// 
// front / back / left / right
// 각각 해당하는 방향에 블록이 존재하는지 확인
// get만 존재하여 값을 읽어오는것만 가능함
// 
// 
// 메서드=========================================================================================
// GetPlayerInputs() 
// 마우스 및 키보드 입력값 확인
// 
// checkDownSpeed(float downSpeed)
// 중력값만큼 하강해야하는지 아닌지 확인
// 
// checkUpSpeed(float upSpeed)
// checkDownSpeed와 비슷하지만, 플레이어 키를 적용시켜서 점프시 천장에 닿는지 확인하는 메서드
// 
// CalculateVelocity()
// 플레이어의 전체적인 이동 및 충돌판정 계산
// checkDownSpeed, checkUpSpeed 사용
// 
// placeCusorBlocks()
// 카메라 방향으로 레이캐스트 기술 코드를 활용하여 설치할 블럭 위치 하이라이트를 배치
// 
// 
// 

public class Player : MonoBehaviour
{
    private Transform cam;
    public Transform highlightBlock, placeBlock;
    private World world;
    private float horizontal, vertical, mouseHrizontal, mouseVertical, verticalMomentum = 0;
    private Vector3 velocity;
    public float walkSpeed = 3f, gravity = -9.8f, sprintSpeed = 6f, jumpForce = 4f, playerWidth = 0.25f, checkIncrement = 0.1f, reach = 8f;
    private bool jumpRequest;
    public bool isGrounded, isSprinting;
    //public ushort selectedBlockIndex = 1;

    public Toolbar toolbar;

    public bool front
    {
        get
        {
            if (
                world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y, transform.position.z + playerWidth)) ||
                world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z + playerWidth))
               ) return true;
            else return false;
        }
    }
    public bool back
    {
        get
        {
            if (
                world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y, transform.position.z - playerWidth)) ||
                world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z - playerWidth))
               ) return true;
            else return false;
        }
    }
    public bool left
    {
        get
        {
            if (
                world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y, transform.position.z)) ||
                world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + 1f, transform.position.z))
               ) return true;
            else return false;
        }
    }
    public bool right
    {
        get
        {
            if (
                world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y, transform.position.z)) ||
                world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + 1f, transform.position.z))
               ) return true;
            else return false;
        }
    }
    
    private void Start()
    {
        cam = GameObject.Find("Main Camera").transform;
        world = GameObject.Find("World").GetComponent<World>();

        world.inUI = false;
    }

    private void FixedUpdate()
    {
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            world.inUI = !world.inUI;
        }

        if (!world.inUI)
        {
            GetPlayerInputs();
            placeCusorBlocks();
            if (jumpRequest) jump();

            transform.Rotate(Vector3.up * mouseHrizontal * world.settings.mouseSensitivity); //회전좌표 y는 가로방향 회전이고, 카메라가 오른쪽으로 회전하면 값이 증가, 카메라가 왼쪽으로 회전하면 값이 감소한다.
            cam.Rotate(Vector3.right * -mouseVertical * world.settings.mouseSensitivity); //회전좌표 x는 세로방향 회전이고, 카메라가 아래로 회전하면 값이 증가, 카메라가 위로 회전하면 값이 감소한다.
        }
        
        CalculateVelocity();
        transform.Translate(velocity, Space.World);
    }

    private void jump()
    {
        verticalMomentum = jumpForce;
        isGrounded = false;
        jumpRequest = false;
    }

    private void CalculateVelocity()
    {
        if (verticalMomentum > gravity) verticalMomentum += Time.deltaTime * gravity;

        velocity += Vector3.up * verticalMomentum * Time.deltaTime;

        if (!world.inUI)
        {
            if (isSprinting) velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.deltaTime * sprintSpeed + (transform.up * verticalMomentum) * Time.deltaTime;
            else velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.deltaTime * walkSpeed + (transform.up * verticalMomentum) * Time.deltaTime;

            if ((velocity.z > 0 && front) || (velocity.z < 0 && back)) velocity.z = 0;
            if ((velocity.x > 0 && right) || (velocity.x < 0 && left)) velocity.x = 0;
        }
        else
        {
            if (isSprinting) velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.deltaTime * sprintSpeed + (transform.up * verticalMomentum) * Time.deltaTime;
            else velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.deltaTime * walkSpeed + (transform.up * verticalMomentum) * Time.deltaTime;

            if ((velocity.z > 0 && front) || (velocity.z < 0 && back)) velocity.z = 0;
            if ((velocity.x > 0 && right) || (velocity.x < 0 && left)) velocity.x = 0;

            // 조건문들이 시간을 잡아먹음으로서 지연되는 시간이 조지게 많다 위의 의미없는 조건문이 없으면 속도판별이 너무 거지같음
            velocity.x = 0; 
            velocity.z = 0;
        }

        if (velocity.y < 0) velocity.y = checkDownSpeed(velocity.y);
        else if (velocity.y > 0) velocity.y = checkUpSpeed(velocity.y);
    }

    private void GetPlayerInputs()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        mouseHrizontal = Input.GetAxis("Mouse X");
        mouseVertical = Input.GetAxis("Mouse Y");

        if (Input.GetButtonDown("Sprint")) isSprinting = true;
        if (Input.GetButtonUp("Sprint")) isSprinting = false;

        if (isGrounded && Input.GetButtonDown("Jump")) jumpRequest = true;

        if (highlightBlock.gameObject.activeSelf)
        {
            if (Input.GetMouseButtonDown(0)) world.GetChunkFromVector3(highlightBlock.position).EditVoxel(highlightBlock.position, 0);
            if (Input.GetMouseButtonDown(1))
            {
                if (toolbar.slots[toolbar.slotIndex].HasItem)
                {
                    world.GetChunkFromVector3(placeBlock.position).EditVoxel(placeBlock.position, toolbar.slots[toolbar.slotIndex].itemSlot.stack.id);
                    toolbar.slots[toolbar.slotIndex].itemSlot.Take(1);
                }
            }
        }
    }

    private void placeCusorBlocks()
    {
        float step = checkIncrement;
        Vector3 lastPos = new Vector3();
        while (step < reach)
        {
            Vector3 pos = cam.position + (cam.forward * step);

            if (world.CheckForVoxel(pos))
            {
                highlightBlock.position = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
                placeBlock.position = lastPos;

                highlightBlock.gameObject.SetActive(true);
                placeBlock.gameObject.SetActive(true);

                return;
            }

            lastPos = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
            step += checkIncrement;
        }

        highlightBlock.gameObject.SetActive(false);
        placeBlock.gameObject.SetActive(false);
    }

    private float checkDownSpeed(float downSpeed)
    {
        if (
            world.CheckForVoxel(new Vector3(transform.position.x - playerWidth + 0.1f, transform.position.y + downSpeed, transform.position.z - playerWidth + 0.1f)) ||
            world.CheckForVoxel(new Vector3(transform.position.x + playerWidth - 0.1f, transform.position.y + downSpeed, transform.position.z - playerWidth + 0.1f)) ||
            world.CheckForVoxel(new Vector3(transform.position.x + playerWidth - 0.1f, transform.position.y + downSpeed, transform.position.z + playerWidth - 0.1f)) ||
            world.CheckForVoxel(new Vector3(transform.position.x - playerWidth + 0.1f, transform.position.y + downSpeed, transform.position.z + playerWidth - 0.1f))
           )
        {
            isGrounded = true;
            return 0;
        }
        else
        {
            isGrounded = false;
            return downSpeed;
        }
    }

    private float checkUpSpeed(float upSpeed)
    {
        if (
            world.CheckForVoxel(new Vector3(transform.position.x - playerWidth + 0.1f, transform.position.y + 1.95f + upSpeed, transform.position.z - playerWidth + 0.1f)) ||
            world.CheckForVoxel(new Vector3(transform.position.x + playerWidth - 0.1f, transform.position.y + 1.95f + upSpeed, transform.position.z - playerWidth + 0.1f)) ||
            world.CheckForVoxel(new Vector3(transform.position.x + playerWidth - 0.1f, transform.position.y + 1.95f + upSpeed, transform.position.z + playerWidth - 0.1f)) ||
            world.CheckForVoxel(new Vector3(transform.position.x - playerWidth + 0.1f, transform.position.y + 1.95f + upSpeed, transform.position.z + playerWidth - 0.1f))
           )
            return 0;
        else return upSpeed;
    }
}
