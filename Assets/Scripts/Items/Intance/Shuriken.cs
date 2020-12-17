using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuriken : Item
{
    public override void OffEquipThis(SlotType offSlot)
    {
        switch (offSlot)
        {
            case SlotType.Accessory:
                {

                }
                break;

            case SlotType.Weapon:
                {

                }
                break;
        }
    }

    public override void OnEquipThis(SlotType onSlot)
    {
        switch (onSlot)
        {
            case SlotType.Accessory:
                {

                }
                break;

            case SlotType.Weapon:
                {

                }
                break;
        }
    }
}
