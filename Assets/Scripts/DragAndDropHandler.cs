using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// DragAndDropHandler
// ���� �巡�� �� ����� ����ϴ� �ڵ鷯 ������Ʈ
// 
// �ڷ���=========================================================================================
// cursorSlot, cursorItemSlot >> Ŀ���� �������� �巡���� �� ó���ϱ� ���� ���콺 ���� ������ĭ( + UIĭ)
// m_Raycaster                   >> 2D UI���� ���콺�� Ŭ������ ��, ��� ������Ʈ�� Ŭ���ߴ���, �޸��� ������Ʈ�� Ŭ����ų���� ���� �����ϱ� ���� ����ĳ��Ʈ
// m_PointerEventData        >> ���콺�� Ŭ�� ���� �̺�Ʈ�� �߻��Ͽ��� ��, �ش� �̺�Ʈ�� ���õ� ����
// m_EventSystem              >> ĵ���� UI���� �Ͼ �� �ִ� ����ĳ��Ʈ
// 
// 
// �޼���=========================================================================================
// CheckForSlot()
// ���콺 �������� ��ġ�� �����ϴ� �޼����, �ش� ��ġ�� �±װ� �����۽����� ������Ʈ�� �ִٸ� �̸� ��ȯ�Ѵ�.
// �翬�ϰԵ�, �����۽����� ������ �ش� �±װ� �پ��־�� �Ѵ�
// 
// HandleSlotClick(UIItemSlot clickedSlot)
// ������ Ŭ���ϸ� ���콺 Ŀ���� �������� �����ϴ� �޼���
// ��ĭ Ŭ����, �������� �ִ� ĭ Ŭ���� �ΰ��� * Ŀ�� ������ ������, Ŀ�� ������ ������ �ΰ���
// 
// 
// 

public class DragAndDropHandler : MonoBehaviour
{
    [SerializeField] private UIItemSlot cursorSlot = null;
    private ItemSlot cursorItemSlot;

    [SerializeField] private GraphicRaycaster m_Raycaster = null;
    private PointerEventData m_PointerEventData;
    [SerializeField] private EventSystem m_EventSystem = null;

    World world;

    // Start is called before the first frame update
    void Start()
    {
        world = GameObject.Find("World").GetComponent<World>();

        cursorItemSlot = new ItemSlot(cursorSlot);
    }

    // Update is called once per frame
    void Update()
    {
        if (!world.inUI) return;

        cursorSlot.transform.position = Input.mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
            HandleSlotClick(CheckForSlot());
        }
    }

    private void HandleSlotClick(UIItemSlot clickedSlot)
    {
        if (clickedSlot == null) return;

        if (!cursorSlot.HasItem && !clickedSlot.HasItem) return;

        if (clickedSlot.itemSlot.isCreative)
        {
            cursorItemSlot.EmptySlot();
            cursorItemSlot.InsertStack(clickedSlot.itemSlot.stack);
            return;
        }

        if (!cursorSlot.HasItem && clickedSlot.HasItem)
        {
            cursorItemSlot.InsertStack(clickedSlot.itemSlot.TakeAll());
            return;
        }

        if (cursorSlot.HasItem && !clickedSlot.HasItem)
        {
            clickedSlot.itemSlot.InsertStack(cursorItemSlot.TakeAll());
            return;
        }

        if (cursorSlot.HasItem && clickedSlot.HasItem)
        {
            if(cursorSlot.itemSlot.stack.id != clickedSlot.itemSlot.stack.id)
            {
                ItemStack oldCursorStack = cursorSlot.itemSlot.TakeAll();
                ItemStack oldSlot = clickedSlot.itemSlot.TakeAll();

                clickedSlot.itemSlot.InsertStack(oldCursorStack);
                cursorSlot.itemSlot.InsertStack(oldSlot);
            }
            return;
        }

    }

    private UIItemSlot CheckForSlot()
    {
        m_PointerEventData = new PointerEventData(m_EventSystem);
        m_PointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        m_Raycaster.Raycast(m_PointerEventData, results);

        foreach(RaycastResult result in results)
        {
            if (result.gameObject.tag == "UIItemSlot") return result.gameObject.GetComponent<UIItemSlot>();
        }

        return null;
    }
}
