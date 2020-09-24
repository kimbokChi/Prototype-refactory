using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golden_Flip : Item
{
    public override float WeaponRange => 3.0f;

    public override ItemRating Rating => ItemRating.Legendary;

    public override void OnEquipThis(SlotType onSlot)
    {
        switch (onSlot)
        {
            case SlotType.Accessory:
                Inventory.Instance.ChargeAction += Charge;               
                break;

            case SlotType.Weapon:
                Inventory.Instance.AttackAction += Combat;
                Inventory.Instance.BeDamagedAction += BeDamaged;
                break;
        }
    }

    public override void OffEquipThis(SlotType offSlot)
    {
        switch (offSlot)
        {
            case SlotType.Accessory:
                Inventory.Instance.ChargeAction -= Charge;
                break;

            case SlotType.Weapon:
                Inventory.Instance.AttackAction -= Combat;
                Inventory.Instance.BeDamagedAction -= BeDamaged;
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

    private void Combat(GameObject attacker, ICombatable targetCombat)
    {
        targetCombat.Damaged(100f, attacker);
    }
}
