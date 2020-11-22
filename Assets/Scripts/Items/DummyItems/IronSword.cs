using System;
using UnityEngine;

public class IronSword : Item
{
    public override void OffEquipThis(SlotType offSlot)
    {
        switch (offSlot)
        {
            case SlotType.Accessory:
                Inventory.Instance.ChargeAction -= ChargeAction;
                break;

            case SlotType.Weapon:
                Inventory.Instance.AttackEvent -= AttackAction;
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
                Inventory.Instance.AttackEvent += AttackAction;
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
        combat.Damaged(float.Parse(JsonString("AttackPower")), attacker);
    }

    private string JsonString(string dataName) {
        return DataUtil.GetDataValue("ItemData", "ID", "IronSword", dataName);
    }
}