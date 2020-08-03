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

    public override void OnEquipThis(SLOT_TYPE onSlot)
    {
        switch (onSlot)
        {
            case SLOT_TYPE.ACCESSORY:
                Inventory.Instnace.ChargeAction += Charge;               
                break;

            case SLOT_TYPE.WEAPON:
                Inventory.Instnace.StruckAction += Combat;
                Inventory.Instnace.BeDamagedAction += BeDamaged;
                break;
        }
    }

    private void Combat(GameObject attacker, ICombat targetCombat)
    {
        targetCombat.Damaged(100f, attacker, out GameObject v);
    }

    public override void OffEquipThis(SLOT_TYPE offSlot)
    {
        switch (offSlot)
        {
            case SLOT_TYPE.ACCESSORY:
                Inventory.Instnace.ChargeAction -= Charge;
                break;

            case SLOT_TYPE.WEAPON:
                Inventory.Instnace.StruckAction -= Combat;
                Inventory.Instnace.BeDamagedAction -= BeDamaged;
                break;
        }
    }
    private void BeDamaged(ref float damage, GameObject attacker, GameObject victim)
    {
        Debug.Log($"Before Damage : {damage}");

        damage *= 0.5f;

        Debug.Log($"After Damage : {damage}");
    }

    private void Charge(float power)
    {
        Debug.Log($"추충전 : {(int)(power * 100)}%");
    }
}
