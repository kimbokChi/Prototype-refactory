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

    public    Sprite  Sprite
    {
        get
        {
            if (mSprite == null)
            {
                if (TryGetComponent(out SpriteRenderer renderer))
                {
                    mSprite = renderer.sprite;
                }
            }
            return mSprite;
        }
    }
    protected Sprite mSprite;

    public abstract void Init();

    public abstract void AccessoryUse(ITEM_KEYWORD keyword);
    public abstract void WeaponUse(ITEM_KEYWORD keyword);
}
