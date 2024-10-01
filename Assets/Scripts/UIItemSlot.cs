using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// UIItemSlot
// ���ٿ� ������ �����۽����� ����ϴ� ���۳�Ʈ
// 
// �ڷ���=========================================================================================
// isLinked      >> �ش� ĭ�� �������� �Ҵ�Ǿ��°�
// itemSlot      >> ȭ�鿡 ������ �ʴ� �������� ����(����뵵)
// slotImage   >> ���� ���� �޹��
// slotIcon      >> ���ٿ� ǥ�õǴ� �̹��� ������
// slotAmount >> �������� ����
// 
// HasItem     >> �ش� �������� ������ �ִ���?
// - get = itemSlot.HasItem
// ��ũ�Ǿ��ִ� �����۽����� �������� �������ִ���
// ���� �����۽����� ���ٰ� �Ѵٸ� �׳� false��ȯ
// 
// 
// �޼���=========================================================================================
// Link(ItemSlot _itemSlot)
// �ڽ��� _itemSlot�� ��ũ��Ų��
// 
// UnLink()
// �ڱ��ڽ� ��ũ����
// 
// UpdateSlot()
// ���ٰ� ��ũ�Ǿ��ִٸ� ���̰� �ϰ�, ����Ǿ����� ������ �Ⱥ��̰�
// ��� ��� �����۽����� �ʱ�ȭ��Ű�� �����̶� �ش� �����۽��Կ� �����ΰ� ������ �ִٸ� ������ �ٲپ��־����
// 
// Clear()
// ������ ���ٰ� �Ⱥ��̰Բ� �ϴ� ����
// 
// OnDestory()
// ���ٰ� ��ũ�Ǿ��ִٸ� �ٷ� ��ũ����
// 
// 
// Ŭ����=========================================================================================
// ItemSlot
// ���ٿ� ���� �����۽��� Ŭ����
// 
// �ڷ���=========================================================================================
// stack         >> �������� �׾Ƴ��� �� �ִ� ����
// uiItemSlot >> ������ ������ ����(����뵵)
// 
// HasItem    >> �������� �����ϸ� Ʈ�� �ƴϸ� ����
// 
// 
// �޼���=========================================================================================
// LinkUISlot(UIItemSlot uiSlot)
// �ڽ��� ���� ����Ǿ������� uiSlot���� ����
// 
// unLickUISlot()
// �ڽŰ� ���� ���� ����
// 
// EmptySlot()
// ������ ���� ����...?
// 
// Take(ushort amt)
// amt������ŭ �ش� �����۽��Կ��� �������� �����ϴ� �õ��� �ϴ� �޼���
// 
// TakeAll()
// Take()�� ��ü��������
// 
// InsertStack(ItemStack _stack)
// _stack��(���� ������ ����)�� �ڱ� �ڽ� �����۽��Կ� �ݿ�
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
