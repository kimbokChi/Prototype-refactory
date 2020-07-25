using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golden_Flip : Item
{
    public override ITEM_RATING RATING
    {
        get
        {
            return ITEM_RATING.LEGENDARY;
        }
    }

    public override float WeaponRange
    {
        get
        {
            return 3.0f;
        }
    }

    public override void Init()
    {
    }

    public override void AccessoryUse(ITEM_KEYWORD keyword)
    {
        Debug.Log($"Using Accessory {keyword}");
    }

    public override void WeaponUse(ITEM_KEYWORD keyword)
    {
        Debug.Log($"Using Weapon {keyword}");
    }
}
