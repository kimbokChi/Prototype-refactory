using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliver_Flip : Item
{
    public override ItemRating Rating => ItemRating.Epic;

    public override void OffEquipThis(SLOT_TYPE offSlot)
    {
        switch (offSlot)
        {
            case SLOT_TYPE.ACCESSORY:
                Inventory.Instance.MoveEndAction -= MoveEndAction;
                break;

            case SLOT_TYPE.WEAPON:
                Inventory.Instance.MoveBeginAction -= MoveBeginAction;
                break;
        }
    }

    public override void OnEquipThis(SLOT_TYPE onSlot)
    {
        switch (onSlot)
        {
            case SLOT_TYPE.ACCESSORY:
                Inventory.Instance.MoveEndAction += MoveEndAction;
                break;

            case SLOT_TYPE.WEAPON:
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
