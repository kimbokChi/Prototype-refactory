using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golden_Flip : Item
{
    public override void Init()
    {
        mWeaponRange = 3.0f;
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
