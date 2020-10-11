using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Ability
{
    MoveSpeed, IMoveSpeed, CurHealth, MaxHealth,
    AttackPower, IAttackPower,
    Begin_AttackDelay, IBegin_AttackDelay,
    After_AttackDelay, IAfter_AttackDelay,
    Range, IRange,
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

    public  RecognitionArea  Area
    {
        get
        {
            if (mArea.Equals(RecognitionArea.Default))
            {
                Init();
            }
            return mArea;
        }
    }
    private RecognitionArea mArea;

    private void Reset() => _JsonTableName = "CharacterAbility";

    public bool CanRecognize(MESSAGE message)
    {
        switch (message)
        {
            case MESSAGE.THIS_ROOM:
                return Area.Equals(RecognitionArea.Room);

            case MESSAGE.BELONG_FLOOR:
                return Area.Equals(RecognitionArea.Floor);
        }
        return false;
    }

    private void Init()
    {
        mTable = new Dictionary<Ability, float>();

        string JsonData(string s)
            => DataUtil.GetDataValue(_JsonTableName, "ID", _JsonLableName, s);

        mArea = (RecognitionArea)Enum.Parse(typeof(RecognitionArea), 
            JsonData("RecognitionArea"));

        for (Ability i = 0; i < Ability.End; ++i)
        {
            string abilityName = i.ToString();
                       
            if (abilityName[0].Equals('I')) {
                mTable.Add(i, default); 
            }
            else
            {
                if (i.Equals(Ability.CurHealth))
                {
                     mTable.Add(i, float.Parse(JsonData("MaxHealth")));
                }
                else mTable.Add(i, float.Parse(JsonData(abilityName)));
            }
        }
    }

    public float MoveSpeed
    { get => Table[Ability.MoveSpeed] + Table[Ability.IMoveSpeed]; }
    public float AttackPower
    { get => Table[Ability.AttackPower] + Table[Ability.IAttackPower]; }
    public float BeginAttackDelay
    { get => Table[Ability.Begin_AttackDelay] + Table[Ability.IBegin_AttackDelay]; }
    public float AfterAttackDelay
    { get => Table[Ability.After_AttackDelay] + Table[Ability.IAfter_AttackDelay]; }

    [SerializeField] private string _JsonTableName;
    [SerializeField] private string _JsonLableName;
}
