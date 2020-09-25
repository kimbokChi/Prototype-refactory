using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Ability
{
    MoveSpeed, IMoveSpeed, CurHealth, MaxHealth,
    AttackPower, IAttackPower, 
    BeginAttackDelay, IBeginAttackDelay,
    AfterAttackDelay, IAfterAttackDelay,
    End
}
public class AbilityTable : MonoBehaviour
{
    public  Dictionary<Ability, float>  Table
    {
        get
        {
            if (mTable == null) { Init(); }

            return mTable;
        }
    }
    private Dictionary<Ability, float> mTable;
    private void Init()
    {
        mTable = new Dictionary<Ability, float>();

        for (Ability i = 0; i < Ability.End; ++i)
        {
            switch (i)
            {
                case Ability.CurHealth:
                case Ability.MaxHealth:
                    mTable.Add(i, _MaxHealth);
                    break;
                case Ability.AttackPower:
                    mTable.Add(i, _AttackPower);
                    break;
                case Ability.BeginAttackDelay:
                    mTable.Add(i, _BeginAttackDelay);
                    break;
                case Ability.AfterAttackDelay:
                    mTable.Add(i, _AfterAttackDelay);
                    break;
                case Ability.MoveSpeed: 
                    mTable.Add(i, _MoveSpeed);
                    break;
                default:
                    mTable.Add(i, default);
                    break;
            }
        }
    }

    public float MoveSpeed
    { get => Table[Ability.MoveSpeed] + Table[Ability.IMoveSpeed]; }
    public float AttackPower
    { get => Table[Ability.AttackPower] + Table[Ability.IAttackPower]; }
    public float BeginAttackDelay
    { get => Table[Ability.BeginAttackDelay] + Table[Ability.IBeginAttackDelay]; }
    public float AfterAttackDelay
    { get => Table[Ability.AfterAttackDelay] + Table[Ability.IAfterAttackDelay]; }

    [SerializeField] private float _MoveSpeed;
    [SerializeField] private float _MaxHealth;
    [SerializeField] private float _AttackPower;

    [SerializeField] private float _BeginAttackDelay;
    [SerializeField] private float _AfterAttackDelay;
}
