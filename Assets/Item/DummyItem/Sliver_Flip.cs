using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliver_Flip : Item
{
    public override ITEM_RATING RATING => ITEM_RATING.EPIC;

    public override ITEM_DATA DATA => ITEM_DATA.SLIVER_FLIP;

    public override void Init()
    {
    }

    public override void OffEquipThis(SLOT_TYPE offSlot)
    {
    }

    public override void OnEquipThis(SLOT_TYPE onSlot)
    {
    }
}
