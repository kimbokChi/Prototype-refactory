using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : Singleton<Inventory>
{
    #region Item Function Event

    #region COMMENT
    /// <summary>
    /// parameter[1] : move direction
    /// </summary>
    #endregion
    public event Action<Vector2> MoveBeginAction;

    #region COMMENT
    /// <summary>
    /// parameter[1] : collision objects when the moving
    /// </summary>
    #endregion
    public event Action<Collider2D[]> MoveEndAction;

    #region COMMENT
    /// <summary>
    /// parameter[1] : attacker gameobject
    /// <br></br>
    /// parameter[2] : attack target interface 'ICombatable'
    /// </summary>
    #endregion
    public event Action<GameObject, ICombatable> AttackAction;

    #region COMMENT
    /// <summary>
    /// parameter[1] : damage
    /// <br></br>
    /// parameter[2] : attacker gameobject
    /// <br></br>
    /// parameter[3] : target gameobject
    /// </summary>
    #endregion
    public event action BeDamagedAction;
    public delegate void action(ref float damage, GameObject attacker, GameObject victim);

    public event Action FloorEnterAction;

    #region COMMENT
    /// <summary>
    /// parameter[1] : charge amount
    /// </summary>
    #endregion
    public event Action<float> ChargeAction;

    #endregion

    public event Action<Item> WeaponChangeEvent
    {
        add    => mWeaponSlot.ItemChangeEvent += value;
        remove => mWeaponSlot.ItemChangeEvent -= value;
    }
    public event Action<Item> WeaponEquipEvent
    {
        add    => mWeaponSlot.ItemEquipEvent += value;
        remove => mWeaponSlot.ItemEquipEvent -= value;
    }

    [SerializeField] private ItemSlot   mWeaponSlot;
    [SerializeField] private ItemSlot[] mAccessorySlot;
    [SerializeField] private ItemSlot[] mContainer;

    private void Awake()
    {
        mWeaponSlot.Init(SlotType.Weapon);

        for (int i = 0; i < mContainer.Length; ++i)
        {
            mContainer[i].Init(SlotType.Container);

            if (i < mAccessorySlot.Length)
            {
                mAccessorySlot[i].Init(SlotType.Accessory);
            }
        }
    }

    public void AddItem(Item item)
    {
        mContainer.Where(o => o.ContainItem == null).First()?.SetItem(item);
    }
    public void Clear()
    {
        mWeaponSlot.SetItem(null);

        mAccessorySlot.ToList().ForEach(o => o.SetItem(null));
            mContainer.ToList().ForEach(o => o.SetItem(null));
    }
    public void OnDamaged(ref float damage, GameObject attacker, GameObject victim)
    {
        BeDamagedAction?.Invoke(ref damage, attacker, victim);
    }

    public void OnCharge(float power)
    {
        ChargeAction?.Invoke(power);
    }

    public void OnAttack(GameObject attacker, ICombatable targetCombat)
    {
        AttackAction?.Invoke(attacker, targetCombat);
    }

    public void OnFloorEnter() => FloorEnterAction?.Invoke();

    public void OnMoveBegin(Vector2 moveDir) => MoveBeginAction?.Invoke(moveDir);
    public void OnMoveEnd(Collider2D[] colliders) => MoveEndAction?.Invoke(colliders);
}