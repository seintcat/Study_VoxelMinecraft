using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// UIItemSlot
// 툴바에 나오는 아이템슬롯을 담당하는 컴퍼넌트
// 
// 자료형=========================================================================================
// isLinked      >> 해당 칸에 아이템이 할당되었는가
// itemSlot      >> 화면에 나오지 않는 아이템의 정보(연결용도)
// slotImage   >> 툴바 슬롯 뒷배경
// slotIcon      >> 툴바에 표시되는 이미지 아이콘
// slotAmount >> 아이템의 수량
// 
// HasItem     >> 해당 아이템을 가지고 있는지?
// - get = itemSlot.HasItem
// 링크되어있는 아이템슬롯이 아이템을 가지고있는지
// 만약 아이템슬롯이 없다고 한다면 그냥 false반환
// 
// 
// 메서드=========================================================================================
// Link(ItemSlot _itemSlot)
// 자신을 _itemSlot에 링크시킨다
// 
// UnLink()
// 자기자신 링크해제
// 
// UpdateSlot()
// 툴바가 링크되어있다면 보이게 하고, 연결되어있지 않으면 안보이게
// 사실 계속 아이템슬롯을 초기화시키는 역할이라 해당 아이템슬롯에 무엇인가 변동이 있다면 무조건 바꾸어주어야함
// 
// Clear()
// 실제로 툴바가 안보이게끔 하는 역할
// 
// OnDestory()
// 툴바가 링크되어있다면 바로 링크해제
// 
// 
// 클래스=========================================================================================
// ItemSlot
// 툴바에 없는 아이템슬롯 클래스
// 
// 자료형=========================================================================================
// stack         >> 아이템을 쌓아놓을 수 있는 수량
// uiItemSlot >> 툴바의 아이템 슬롯(연결용도)
// 
// HasItem    >> 아이템이 존재하면 트루 아니면 폴즈
// 
// 
// 메서드=========================================================================================
// LinkUISlot(UIItemSlot uiSlot)
// 자신이 어디와 연결되었는지를 uiSlot으로 저장
// 
// unLickUISlot()
// 자신과 툴바 연결 해제
// 
// EmptySlot()
// 아이템 연결 해제...?
// 
// Take(ushort amt)
// amt수량만큼 해당 아이템슬롯에서 아이템을 제거하는 시도를 하는 메서드
// 
// TakeAll()
// Take()의 전체수량버전
// 
// InsertStack(ItemStack _stack)
// _stack값(실제 아이템 정보)를 자기 자신 아이템슬롯에 반영
// 
// 


public class UIItemSlot : MonoBehaviour
{
    public bool isLinked = false;
    public ItemSlot itemSlot;
    public Image slotImage, slotIcon;
    public Text slotAmount;

    World world;

    private void Awake()
    {
        world = GameObject.Find("World").GetComponent<World>();
    }

    public bool HasItem
    {
        get
        {
            if (itemSlot == null) return false;
            else return itemSlot.HasItem;
        }
    }

    public void Link(ItemSlot _itemSlot)
    {
        itemSlot = _itemSlot;
        isLinked = true;
        itemSlot.LinkUISlot(this);

        UpdateSlot();
    }

    public void UnLink()
    {
        itemSlot.unLinckUISlot();
        //isLinked = false;
        itemSlot = null;

        UpdateSlot();
    }

    public void UpdateSlot()
    {
        if (itemSlot != null && itemSlot.HasItem)
        {
            slotIcon.sprite = world.blockTypes[itemSlot.stack.id].icon;
            slotAmount.text = itemSlot.stack.amount.ToString();
            slotIcon.enabled = true;
            slotAmount.enabled = true;
        }
        else Clear();
    }

    public void Clear()
    {
        slotIcon.sprite = null;
        slotAmount.text = "";
        slotIcon.enabled = false;
        slotAmount.enabled = false;
    }

    private void OnDestory()
    {
        if (isLinked) itemSlot.unLinckUISlot();
    }
}

public class ItemSlot
{
    public ItemStack stack = null;
    private UIItemSlot uiItemSlot = null;

    public bool isCreative;

    public ItemSlot(UIItemSlot _uiItemSlot)
    {
        stack = null;
        uiItemSlot = _uiItemSlot;
        uiItemSlot.Link(this);
    }

    public ItemSlot(UIItemSlot _uiItemSlot, ItemStack _stack)
    {
        stack = _stack;
        uiItemSlot = _uiItemSlot;
        uiItemSlot.Link(this);
    }

    public void LinkUISlot(UIItemSlot uiSlot)
    {
        uiItemSlot = uiSlot;
    }

    public void unLinckUISlot()
    {
        uiItemSlot = null;
    }

    public void EmptySlot()
    {
        stack = null;
        if (uiItemSlot != null) uiItemSlot.UpdateSlot();
    }

    public ushort Take(ushort amt)
    {
        if (amt > stack.amount)
        {
            ushort amount = stack.amount;
            EmptySlot();
            return amount;
        }
        else if(amt < stack.amount)
        {
            stack.amount -= amt;
            uiItemSlot.UpdateSlot();
            return amt;
        }
        else
        {
            EmptySlot();
            return amt;
        }
    }

    public ItemStack TakeAll()
    {
        ItemStack handOver = new ItemStack(stack.id, stack.amount);
        EmptySlot();
        return handOver;
    }

    public void InsertStack(ItemStack _stack)
    {
        stack = _stack;
        uiItemSlot.UpdateSlot();
    }

    public bool HasItem
    {
        get
        {
            if (stack != null) return true;
            else return false;
        }
    }
}
