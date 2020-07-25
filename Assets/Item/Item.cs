using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public    float  WeaponRange
    {
        get { return mWeaponRange; }
    }
    protected float mWeaponRange = 0.0f;

    public abstract void Init();

    public abstract void AccessoryUse(ITEM_KEYWORD keyword);
    public abstract void WeaponUse(ITEM_KEYWORD keyword);
}
