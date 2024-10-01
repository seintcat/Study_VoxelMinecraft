using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// DragAndDropHandler
// 물건 드래그 앤 드롭을 담당하는 핸들러 컴포넌트
// 
// 자료형=========================================================================================
// cursorSlot, cursorItemSlot >> 커서로 아이템을 드래그할 때 처리하기 위한 마우스 전용 아이템칸( + UI칸)
// m_Raycaster                   >> 2D UI에서 마우스로 클릭했을 때, 어느 오브젝트를 클릭했는지, 뒷면의 오브젝트도 클릭시킬건지 등을 구분하기 위한 레이캐스트
// m_PointerEventData        >> 마우스로 클릭 등의 이벤트가 발생하였을 때, 해당 이벤트에 관련된 정보
// m_EventSystem              >> 캔버스 UI에서 일어날 수 있는 레이캐스트
// 
// 
// 메서드=========================================================================================
// CheckForSlot()
// 마우스 포인터의 위치를 검증하는 메서드로, 해당 위치에 태그가 아이템슬롯인 오브젝트가 있다면 이를 반환한다.
// 당연하게도, 아이템슬롯은 무조건 해당 태그가 붙어있어야 한다
// 
// HandleSlotClick(UIItemSlot clickedSlot)
// 슬롯을 클릭하면 마우스 커서에 아이템을 전달하는 메서드
// 빈칸 클릭시, 아이템이 있는 칸 클릭시 두가지 * 커서 아이템 없을시, 커서 아이템 있을시 두가지
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
