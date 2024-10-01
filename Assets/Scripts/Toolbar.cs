using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Toolbar
// 툴바 UI 스크립트
// 
// 자료형=========================================================================================
// slots >> 아이템슬롯 배열
// highlight                      >> 툴바 아이템 선택표시 이미지
// player                          >> 플레이어 컴포넌트
// slotIndex                     >> 현재 툴바에서 선택하고있는 아이템 위치 인덱스
// 
// 
// 
// 메서드=========================================================================================
// 
// 
// 
// 
// 


public class Toolbar : MonoBehaviour
{

    public UIItemSlot[] slots;
    public RectTransform highlight;
    public Player player;
    public int slotIndex = 0;

    private void Start()
    {
        ushort index = 1;
        foreach(UIItemSlot s in slots)
        {
            ItemStack stack = new ItemStack(index, (ushort)Random.Range(2, 65));
            ItemSlot slot = new ItemSlot(s, stack);
            if(index < 8) index++;
        }
    }

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            if (scroll > 0) slotIndex--;
            else slotIndex++;

            if (slotIndex > slots.Length - 1) slotIndex = 0;
            else if (slotIndex < 0) slotIndex = slots.Length - 1;

            highlight.position = slots[slotIndex].slotIcon.transform.position;
            //if (slots[slotIndex].itemSlot != null) player.selectedBlockIndex = slots[slotIndex].itemSlot.stack.id;
            //else player.selectedBlockIndex = 0;

        }
    }
}


//[System.Serializable]

//public class ItemSlot
//{
//    public ushort itemID;
//    public Image Icon;
//}

//// ItemSlot 클래스
//// itemID                         >> 툴바에 존재하는 아이템의 ID
//// Icon                             >> 툴바에 존재하는 아이템의 아이콘

//    World world;
//    public ItemSlot[] itemSlots;

//    private void Start()
//    {
//        world = GameObject.Find("World").GetComponent<World>();

//        foreach(ItemSlot slot in itemSlots)
//        {
//            if(slot.itemID != 0)
//            {
//                slot.Icon.sprite = world.blockTypes[slot.itemID].icon;
//                slot.Icon.enabled = true;
//            }
//            else slot.Icon.enabled = false;
//        }
//        player.selectedBlockIndex = itemSlots[slotIndex].itemID;
//    }

////world >> 월드 컴포넌트
//// itemSlots                     >> 툴바에 존재하는 아이템슬롯의 배열
