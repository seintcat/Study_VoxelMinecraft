using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// CreativeInventory
// ũ������Ƽ���� �κ��丮â ������ ���� ������Ʈ
// 
// �ڷ���=========================================================================================
// slotPrefab  >> ũ������Ƽ�� �κ��丮â���� �ڵ����� ������ĭ�� �����ϱ� ���� �����۽���
// world         >> ���� ������Ʈ
// slots          >> ũ������Ƽ�� ������ĭ�� �����ϱ� ���� ���� ����Ʈ
// 
// 
// 
// �޼���=========================================================================================
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
