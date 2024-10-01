using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Toolbar
// ���� UI ��ũ��Ʈ
// 
// �ڷ���=========================================================================================
// slots >> �����۽��� �迭
// highlight                      >> ���� ������ ����ǥ�� �̹���
// player                          >> �÷��̾� ������Ʈ
// slotIndex                     >> ���� ���ٿ��� �����ϰ��ִ� ������ ��ġ �ε���
// 
// 
// 
// �޼���=========================================================================================
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

//// ItemSlot Ŭ����
//// itemID                         >> ���ٿ� �����ϴ� �������� ID
//// Icon                             >> ���ٿ� �����ϴ� �������� ������

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

////world >> ���� ������Ʈ
//// itemSlots                     >> ���ٿ� �����ϴ� �����۽����� �迭
