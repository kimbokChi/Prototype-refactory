using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public enum SlotType
{
    Container, Accessory, Weapon
}
public class ItemSlot : MonoBehaviour
{
    public event Action<Item> ItemChangeEvent;
    public event Action<Item> ItemEquipEvent;

    public  Item  ContainItem => mContainItem;
    private Item mContainItem;
    
    private Image mImage;

    private SlotType mSlotType;

    public void Init(SlotType type)
    {
        mSlotType = type;

        TryGetComponent(out mImage);
    }

    public void SetItem(Item item)
    {
        if (mContainItem != null)
        {
            ItemChangeEvent?.Invoke(mContainItem);

            mContainItem.OffEquipThis(mSlotType);
        }        
        mContainItem = item;

        ItemEquipEvent?.Invoke(mContainItem);

        if (item == null)
        {
            mImage.sprite = null;
        }
        else
        {
            item.OnEquipThis(mSlotType);

            mImage.sprite = item.Sprite;
        }
    }

    public void Select()
    {
        Item fingerCarryItem = Finger.Instance.CarryItem;

        Finger.Instance.CarryItem = mContainItem;
        mContainItem = fingerCarryItem;

        SetItem(mContainItem);
    }
}
