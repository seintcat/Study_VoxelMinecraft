using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Player
// �����̴� �÷��̾ ������ ������Ʈ
// 
// �ڷ���=========================================================================================
// cam                                         >> �÷��̾� �þ�(ī�޶�)�� ��ġ����
// world                                       >> �÷��̾��� �浹ü ������ ���� ���� ����
// horizontal / vertical                   >> �÷��̾� �����¿� �̵�Ű ��
// mouseHrizontal / mouseVertical >> ���콺 X��ǥ, ���콺 Y��ǥ��
// velocity                                    >> �÷��̾� Ű���� �Է¿� ���� �̵� ��ǥ��
// walkSpeed                                >> �÷��̾� �̵��ӵ�
// gravity                                     >> �÷��̾� �߷�
// sprintSpeed                              >> �÷��̾� �޸��� �ӵ�
// jumpForce                                >> �÷��̾� ������
// playerWidth                              >> �÷��̾� �β�
// verticalMomentum                     >> ���� �ڵ��� ��� �߷°��ӵ��� ǥ��
// jumpRequest                             >> �÷��̾��� ����Ű �Է� ����
// isGrounded                               >> �÷��̾ ���� ��Ҵ��� ����
// isSprinting                                >> �÷��̾��� �޸���Ű �Է� ����
// highlightBlock                           >> ���ŵǴ� ���� ��ġ
// placeBlock                                >> �����Ǵ� ���� ��ġ
// checkIncrement                        >> ����ĳ��Ʈ �� ���� �����ϴ��� üũ�ϴ� �Ÿ� ����
// reach                                       >> ���� ��ġ�� �� �ִ� �Ÿ�
// selectedBlockText                      >> ���� �տ� ����ִ� �� �̸� UI
// selectedBlockIndex                    >> ����������Ʈ�� ����� �� �ε���
// toolbar                                      >> ����
// 
// 
// front / back / left / right
// ���� �ش��ϴ� ���⿡ ����� �����ϴ��� Ȯ��
// get�� �����Ͽ� ���� �о���°͸� ������
// 
// 
// �޼���=========================================================================================
// GetPlayerInputs() 
// ���콺 �� Ű���� �Է°� Ȯ��
// 
// checkDownSpeed(float downSpeed)
// �߷°���ŭ �ϰ��ؾ��ϴ��� �ƴ��� Ȯ��
// 
// checkUpSpeed(float upSpeed)
// checkDownSpeed�� ���������, �÷��̾� Ű�� ������Ѽ� ������ õ�忡 ����� Ȯ���ϴ� �޼���
// 
// CalculateVelocity()
// �÷��̾��� ��ü���� �̵� �� �浹���� ���
// checkDownSpeed, checkUpSpeed ���
// 
// placeCusorBlocks()
// ī�޶� �������� ����ĳ��Ʈ ��� �ڵ带 Ȱ���Ͽ� ��ġ�� �� ��ġ ���̶���Ʈ�� ��ġ
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

            transform.Rotate(Vector3.up * mouseHrizontal * world.settings.mouseSensitivity); //ȸ����ǥ y�� ���ι��� ȸ���̰�, ī�޶� ���������� ȸ���ϸ� ���� ����, ī�޶� �������� ȸ���ϸ� ���� �����Ѵ�.
            cam.Rotate(Vector3.right * -mouseVertical * world.settings.mouseSensitivity); //ȸ����ǥ x�� ���ι��� ȸ���̰�, ī�޶� �Ʒ��� ȸ���ϸ� ���� ����, ī�޶� ���� ȸ���ϸ� ���� �����Ѵ�.
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

            // ���ǹ����� �ð��� ��Ƹ������μ� �����Ǵ� �ð��� ������ ���� ���� �ǹ̾��� ���ǹ��� ������ �ӵ��Ǻ��� �ʹ� ��������
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
