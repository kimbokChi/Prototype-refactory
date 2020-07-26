using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golden_Flip : Item
{
    public override float WeaponRange => 3.0f;

    public override ITEM_RATING RATING
    {
        get
        {
            return ITEM_RATING.LEGENDARY;
        }
    }

    public override ITEM_DATA DATA
    {
        get
        {
            return ITEM_DATA.GOLDEN_FLIP;
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
