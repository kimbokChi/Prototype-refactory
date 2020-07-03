using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golden_Flip : Item
{
    public override void Init()
    {
        mWeaponRange = 3.0f;
    }

    public override void UseItem(ITEM_KEYWORD keyword)
    {
        Debug.Log(keyword.ToString());
    }
}
