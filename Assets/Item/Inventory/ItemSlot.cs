using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

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
        mContainItem = item;

        if (item == null)
        {
             mImage.sprite = null;
        }
        else mImage.sprite = item.Sprite;
    }

    public void Select()
    {
        Finger.Instnace.GetCarryItem(out Item carryItem);

        Finger.Instnace.SetCarryItem(mContainItem);

        SetItem(carryItem);
    }
}
