using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public enum SlotType
{
    Container, Accessory, Weapon
}
public class ItemSlot : MonoBehaviour
{
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
            mContainItem.OffEquipThis(mSlotType);
        }        
        mContainItem = item;

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
        Item carryItem = Finger.Instance.CarryItem;
          mContainItem = Finger.Instance.CarryItem;

        SetItem(carryItem);
    }
}
