using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public void AddItem(Item item)
    {
        for (int i = 0; i < mContainer.Length; ++i)
        {
            if (mContainer[i].ContainItem == null)
            {
                mContainer[i].SetItem(item);

                return;
            }
        }
        Debug.Log("더이상 아이템을 담을수 없습니다!");
    }

    public void UseItem(ITEM_KEYWORD KEYWORD)
    {
        if (mWeaponSlot.ContainItem != null)
        {
            mWeaponSlot.ContainItem.WeaponUse(KEYWORD);
        }
        for (int i = 0; i < mAccessorySlot.Length; ++i)
        {
            if (mAccessorySlot[i].ContainItem != null)
            {
                mAccessorySlot[i].ContainItem.AccessoryUse(KEYWORD);
            }
        }
    }

    private IEnumerable<ItemSlot> EGetSlots()
    {
        int iterIndex = 0;

        yield return mWeaponSlot;

        while (iterIndex < mAccessorySlot.Length)
        {
            yield return mAccessorySlot[iterIndex++];
        }

        iterIndex = 0;
        
        while (iterIndex < mContainer.Length)
        {
            yield return mContainer[iterIndex++];
        }

        yield break;
    }
}
