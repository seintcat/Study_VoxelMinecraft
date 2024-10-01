using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// CreativeInventory
// 크리에이티브모드 인벤토리창 구현을 위한 컴포넌트
// 
// 자료형=========================================================================================
// slotPrefab  >> 크리에이티브 인벤토리창에서 자동으로 아이템칸을 생성하기 위한 아이템슬롯
// world         >> 월드 컴포넌트
// slots          >> 크리에이티브 아이템칸을 관리하기 위한 슬롯 리스트
// 
// 
// 
// 메서드=========================================================================================
// 
// 
// 

public class CreativeInventory : MonoBehaviour
{
    public GameObject slotPrefab;
    World world;
    List<ItemSlot> slots = new List<ItemSlot>();

    // Start is called before the first frame update
    void Start()
    {
        world = GameObject.Find("World").GetComponent<World>();

        for (ushort i = 1; i < world.blockTypes.Length; i++)
        {
            GameObject newSlot = Instantiate(slotPrefab, transform);

            ItemStack stack = new ItemStack(i, 64);
            ItemSlot slot = new ItemSlot(newSlot.GetComponent<UIItemSlot>(), stack);

            slot.isCreative = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
