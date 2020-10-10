using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Ability
{
    MoveSpeed, IMoveSpeed, CurHealth, MaxHealth,
    AttackPower, IAttackPower, AttackDelay,
    Begin_AttackDelay, IBegin_AttackDelay,
    After_AttackDelay, IAfter_AttackDelay, 
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

    private Func<string, float> GetJsonData;

    private void Init()
    {
        mTable = new Dictionary<Ability, float>();

        GetJsonData = s =>
        {
            return float.Parse(DataUtil.GetDataValue(_JsonTableName, "ID", _JsonLableName, s));
        };

        for (Ability i = 0; i < Ability.End; ++i)
        {
            string abilityName = i.ToString();

            if (abilityName[0].Equals('I')) mTable.Add(i, default);

            else
            {
                if (abilityName.Equals("CurHealth"))
                {
                     mTable.Add(i, GetJsonData("MaxHealth"));
                }
                else mTable.Add(i, GetJsonData(i.ToString()));

                Debug.Log($"{i} : {mTable[i]}");
            }
        }
    }

    public float MoveSpeed
    { get => Table[Ability.MoveSpeed] + Table[Ability.IMoveSpeed]; }
    public float AttackPower
    { get => Table[Ability.AttackPower] + Table[Ability.IAttackPower]; }
    public float AttackDelay
    { get => Table[Ability.AttackDelay]; }
    public float BeginAttackDelay
    { get => Table[Ability.Begin_AttackDelay] + Table[Ability.IBegin_AttackDelay]; }
    public float AfterAttackDelay
    { get => Table[Ability.After_AttackDelay] + Table[Ability.IAfter_AttackDelay]; }


    [SerializeField] private string _JsonTableName;
    [SerializeField] private string _JsonLableName;


    [SerializeField] private float _MoveSpeed;
    [SerializeField] private float _MaxHealth;
    [SerializeField] private float _AttackPower;

    [SerializeField] private float _AttackDelay;
    [SerializeField] private float _BeginAttackDelay;
    [SerializeField] private float _AfterAttackDelay;
}
