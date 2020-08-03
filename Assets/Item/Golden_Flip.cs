﻿using System.Collections;
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
        ItemLibrary.Instnace.BeDamagedAction += BeDamaged;
    }

    private void BeDamaged(ref float damage, GameObject attacker, GameObject victim)
    {
        damage *= 0.5f;
    }

    public override void AccessoryUse(ITEM_KEYWORD keyword)
    {
        if (keyword == ITEM_KEYWORD.ENTER)
        {
            Debug.Log("슨슈-입장-!");
        }
        else Debug.Log($"Using Accessory {keyword}");
    }

    public override void WeaponUse(ITEM_KEYWORD keyword)
    {
        if (keyword == ITEM_KEYWORD.ENTER)
        {
            Debug.Log("슨슈-입장-!");
        }
        else Debug.Log($"Using Weapon {keyword}");
    }

    public override void CAccessoryUse(float power)
    {
        Debug.Log($"Using Accessory Charge : {(int)(power * 100)}%");
    }

    public override void CWeaponUse(float power)
    {
        Debug.Log($"Using Weapon Charge : {(int)(power * 100)}%");
    }
}
