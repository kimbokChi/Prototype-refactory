using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliver_Flip : Item
{
    public override ItemRating Rating => ItemRating.Epic;

    public override void OffEquipThis(SlotType offSlot)
    {
        switch (offSlot)
        {
            case SlotType.Accessory:
                Inventory.Instance.MoveEndAction -= MoveEndAction;
                break;

            case SlotType.Weapon:
                Inventory.Instance.MoveBeginAction -= MoveBeginAction;
                break;
        }
    }

    public override void OnEquipThis(SlotType onSlot)
    {
        switch (onSlot)
        {
            case SlotType.Accessory:
                Inventory.Instance.MoveEndAction += MoveEndAction;
                break;

            case SlotType.Weapon:
                Inventory.Instance.MoveBeginAction += MoveBeginAction;
                break;
        }
    }

    private void MoveBeginAction(Vector2 moveDir)
    {
        Debug.Log($"Move Direction : {moveDir}");
    }

    private void MoveEndAction(Collider2D[] colliders)
    {
        for (int i = 0; i < colliders.Length; ++i)
        {
            if (colliders[i].TryGetComponent(out ICombatable combat))
            {
                combat.Damaged(60, FindObjectOfType(typeof(Player)) as GameObject);
            }
        }
    }
}
