using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : Singleton<Inventory>
{
    public const float DEFAULT_RANGE = 1f;

    #region Item Function Event

    public delegate void UseMoveBegin();
    public event UseMoveBegin MoveBeginAction;

    public delegate void UseMoveEnd();
    public event UseMoveEnd MoveEndAction;

    public delegate void UseStruck();
    public event UseStruck StruckAction;

    public delegate void UseBeDamaged(ref float damage, GameObject attacker, GameObject victim);
    public event UseBeDamaged BeDamagedAction;

    public delegate void UseEnter();
    public event UseEnter EnterAction;

    public delegate void UseCharge(float charge);
    public event UseCharge ChargeAction;

    #endregion

    [SerializeField] private ItemSlot   mWeaponSlot;
    [SerializeField] private ItemSlot[] mAccessorySlot;
    [SerializeField] private ItemSlot[] mContainer;

    private void Awake()
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
        EventInit();
    }

    private void EventInit()
    {
        if (BeDamagedAction == null)
        {
            BeDamagedAction = delegate (ref float damage, GameObject attacker, GameObject victim) { };
        }
        if (ChargeAction == null)
        {
            ChargeAction = delegate (float power) { };
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

    public float GetWeaponRange()
    {
        if (mWeaponSlot.ContainItem != null)
        {
            return mWeaponSlot.ContainItem.WeaponRange;
        }
        return DEFAULT_RANGE;
    }

    public void UseDamagedAction(ref float damage, GameObject attacker, GameObject victim)
    {
        BeDamagedAction.Invoke(ref damage, attacker, victim);
    }

    public void UseChargeAction(float power)
    {
        ChargeAction.Invoke(power);
    }
}
