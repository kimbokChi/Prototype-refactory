using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public enum SLOT_TYPE
{
    CONTAINER, ACCESSORY, WEAPON
}
public class ItemSlot : MonoBehaviour
{
    public  Item  ContainItem => mContainItem;
    private Item mContainItem;
    
    private Image mImage;

    private SLOT_TYPE mSLOT_TYPE;

    public void Init(SLOT_TYPE type)
    {
        mSLOT_TYPE = type;

        TryGetComponent(out mImage);
    }

    public void SetItem(Item item)
    {
        if (mContainItem != null)
        {
            mContainItem.OffEquipThis(mSLOT_TYPE);
        }        
        mContainItem = item;

        if (item == null)
        {
            mImage.sprite = null;
        }
        else
        {
            item.OnEquipThis(mSLOT_TYPE);

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
