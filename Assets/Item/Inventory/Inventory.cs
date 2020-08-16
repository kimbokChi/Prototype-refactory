using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : Singleton<Inventory>
{
    public const float DEFAULT_RANGE = 1f;

    #region Item Function Event

    public delegate void MoveBegin(Vector2 moveDir);
    public event MoveBegin MoveBeginAction;

    public delegate void MoveEnd(Collider2D[] colliders);
    public event MoveEnd MoveEndAction;

    public delegate void Attack(GameObject attacker, ICombat targetCombat);
    public event Attack AttackAction;

    public delegate void BeDamaged(ref float damage, GameObject attacker, GameObject victim);
    public event BeDamaged BeDamagedAction;

    public delegate void FloorEnter();
    public event FloorEnter FloorEnterAction;

    public delegate void Charge(float charge);
    public event Charge ChargeAction;

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
        if (AttackAction == null)
        {
            AttackAction = delegate (GameObject attacker, ICombat targetCombat) { };
        }
        if (MoveBeginAction == null)
        {
            MoveBeginAction = delegate (Vector2 dir) { };
        }
        if (MoveEndAction == null)
        {
            MoveEndAction = delegate (Collider2D[] colliders) { };
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

    public void UseAttackAction(GameObject attacker, ICombat targetCombat)
    {
        AttackAction.Invoke(attacker, targetCombat);
    }

    public void UseMoveBeginAction(Vector2 moveDir) => MoveBeginAction.Invoke(moveDir);
    public void UseMoveEndAction(Collider2D[] colliders) => MoveEndAction.Invoke(colliders);
}
