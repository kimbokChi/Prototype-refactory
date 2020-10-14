using UnityEngine;

public class IronSword : Item
{
    public override ItemRating Rating => ItemRating.Epic;

    public override float WeaponRange => 1.5f;

    public override void OffEquipThis(SlotType offSlot)
    {
        switch (offSlot)
        {
            case SlotType.Accessory:
                Inventory.Instance.ChargeAction -= ChargeAction;
                break;

            case SlotType.Weapon:
                Inventory.Instance.AttackAction -= AttackAction;
                break;
        }
    }

    public override void OnEquipThis(SlotType onSlot)
    {
        switch (onSlot)
        {
            case SlotType.Accessory:
                Inventory.Instance.ChargeAction += ChargeAction;
                break;

            case SlotType.Weapon:
                Inventory.Instance.AttackAction += AttackAction;
                break;
        }
    }

    private void ChargeAction(float charge)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        GameObject player = FindObjectOfType(typeof(Player)) as GameObject;

        for (int i = 0; i < enemies.Length; ++i)
        {
            if (enemies[i].activeSelf)
            {
                if (enemies[i].TryGetComponent(out ICombatable combatable)) {
                    combatable.Damaged(charge * 8f, player);
                }
            }
        }
    }

    private void AttackAction(GameObject attacker, ICombatable combat)
    {
        combat.Damaged(5f, attacker);
    }
}