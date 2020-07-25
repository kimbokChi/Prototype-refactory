using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finger : Singleton<Finger>
{
    private Item mCarryItem = null;

    public void SetCarryItem(Item item)
    {
        mCarryItem = item;
    }
    public void GetCarryItem(out Item item)
    {
        item = mCarryItem;
    }
}
