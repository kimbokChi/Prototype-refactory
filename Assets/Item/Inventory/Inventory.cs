using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private ItemSlot   mWeaponSlot;
    [SerializeField] private ItemSlot[] mAccessorySlot = new ItemSlot[4];
    [SerializeField] private ItemSlot[] mContainer     = new ItemSlot[8];

    public void Init()
    {
        mWeaponSlot.Init(SLOT_TYPE.WEAPON);

        for (int i = 0; i < mContainer.Length; ++i)
        {
            mContainer[i].Init(SLOT_TYPE.CONTAINER);

            if (i < mAccessorySlot.Length)
            {
                mAccessorySlot[i].Init(SLOT_TYPE.ACCESSORY);
            }
        }
    }
}
