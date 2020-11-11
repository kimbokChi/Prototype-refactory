using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public System.Action AttackOverAction;

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
    { get => 1f; }

    public abstract void  OnEquipThis(SlotType onSlot);
    public abstract void OffEquipThis(SlotType offSlot);

    public virtual void AttackAction(GameObject attacker, ICombatable combatable)
    { 
        Inventory.Instance.OnAttackEvent(attacker, combatable); }
}
