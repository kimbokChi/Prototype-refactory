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

    public virtual ItemRating Rating => ItemRating.Common;
    public virtual float WeaponRange
    {
        get 
        {
            return 1.0f; 
        }
    }

    public abstract void  OnEquipThis(SlotType onSlot);
    public abstract void OffEquipThis(SlotType offSlot);
}
