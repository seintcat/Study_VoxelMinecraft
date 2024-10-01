using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ItemStack
// 아이템 쌓기 정보를 저장하는 클래스
// 
// 자료형=========================================================================================
// id          >> 아이템 id
// amount >> 아이템 수량
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
