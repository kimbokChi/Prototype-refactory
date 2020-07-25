using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
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

    public virtual ITEM_RATING RATING
    {
        get
        {
            return ITEM_RATING.COMMON;
        }
    }

    public virtual float WeaponRange
    {
        get 
        {
            return 1.0f; 
        }
    }

    public abstract void Init();

    public abstract void AccessoryUse(ITEM_KEYWORD keyword);
    public abstract void WeaponUse(ITEM_KEYWORD keyword);
}
