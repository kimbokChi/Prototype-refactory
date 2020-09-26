using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class Inventory : Singleton<Inventory>
{
    public const float DEFAULT_RANGE = 1f;

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

    public float GetWeaponRange()
    {
        return (mWeaponSlot.ContainItem != null) ? mWeaponSlot.ContainItem.WeaponRange : DEFAULT_RANGE;
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

    public void OnMoveBegin(Vector2 moveDir) => MoveBeginAction?.Invoke(moveDir);
    public void OnMoveEnd(Collider2D[] colliders) => MoveEndAction?.Invoke(colliders);
}

[CustomEditor(typeof(Inventory))]
public class AddInventory : Editor
{
    private Item mAddTarget;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (Application.isPlaying)
        {
            GUILayout.Space(6f);

            GUILayout.Label("Select Item", EditorStyles.boldLabel);
            mAddTarget = EditorGUILayout.ObjectField(mAddTarget, typeof(Item), true) as Item;

            if (GUILayout.Button("Add Item", GUILayout.Height(20f)))
            {
                Inventory.Instance.AddItem(mAddTarget);
            }
            if (GUILayout.Button("Clear Inventory", GUILayout.Height(20f)))
            {
                Inventory.Instance.Clear();
            }
        }
    }
}