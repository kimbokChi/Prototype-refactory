﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ItemSlot : MonoBehaviour
{
    public  Item  ContainItem
    { get { return mContainItem; } }
    private Item mContainItem;
    
    private Image mImage;

    private SLOT_TYPE mSLOT_TYPE;

    public void Init(SLOT_TYPE type)
    {
        mSLOT_TYPE = type;

        TryGetComponent(out mImage);

        mContainItem = null;
    }

    public void SetItem(Item item)
    {
        mContainItem = item;

        mImage.sprite = item.Sprite;
    }
}
