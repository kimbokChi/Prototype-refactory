using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public System.Action AttackOverAction;

    [SerializeField]
    protected ItemStatTable StatTable;

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

    // *********************************** //
    public ItemRating Rating
    {
        get => StatTable.Rating;
    }
    public float WeaponRange
    { 
        get => StatTable.Table[ItemStat.Range]; 
    }
    public float Begin_AttackDelay
    {
        get => StatTable.Table[ItemStat.Begin_AttackDelay];
    }
    public float After_AttackDelay
    {
        get => StatTable.Table[ItemStat.After_AttackDelay];
    }

    // *********************************** //

    public abstract void  OnEquipThis(SlotType onSlot);
    public abstract void OffEquipThis(SlotType offSlot);

    public virtual void AttackAction(GameObject attacker, ICombatable combatable)
    { 
        Inventory.Instance.OnAttackEvent(attacker, combatable); 
    }
}
