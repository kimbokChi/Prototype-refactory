using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Iron_Flip : Item
{
    public override ITEM_RATING RATING => ITEM_RATING.COMMON;

    public override ITEM_DATA DATA => ITEM_DATA.IRON_FLIP;

    public override void OffEquipThis(SLOT_TYPE offSlot)
    {
    }

    public override void OnEquipThis(SLOT_TYPE onSlot)
    {
    }
}
