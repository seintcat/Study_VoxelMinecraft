using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ItemStack
// ������ �ױ� ������ �����ϴ� Ŭ����
// 
// �ڷ���=========================================================================================
// id          >> ������ id
// amount >> ������ ����
// 

public class ItemStack
{
    public ushort id;
    public ushort amount;

    public ItemStack(ushort _id, ushort _amount)
    {
        id = _id;
        amount = _amount;
    }
}
