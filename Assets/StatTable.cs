using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum STAT_ON_TABLE
{
    MOVESPEED, IMOVESPEED,
    CURHEALTH, MAXHEALTH,
    OFFENSIVE_POWER, IOFFENSIVE_POWER
}

public class StatTable : MonoBehaviour
{
    private int KeyCode;
    private Dictionary<STAT_ON_TABLE, float> mStatTable;

    [SerializeField]
    private GameObject BelongObject;

    private void Reset()
    {
        BelongObject = transform.parent.gameObject;
    }

    private void Awake()
    {
        KeyCode = BelongObject.GetHashCode();

        mStatTable = new Dictionary<STAT_ON_TABLE, float>();

        float increase = 0f;

        mStatTable.Add(STAT_ON_TABLE. MOVESPEED,  mMoveSpeed);
        mStatTable.Add(STAT_ON_TABLE.IMOVESPEED,    increase);

        mStatTable.Add(STAT_ON_TABLE.CURHEALTH, mMaxHealth);
        mStatTable.Add(STAT_ON_TABLE.MAXHEALTH, mMaxHealth);

        mStatTable.Add(STAT_ON_TABLE. OFFENSIVE_POWER, mOffensivePower);
        mStatTable.Add(STAT_ON_TABLE.IOFFENSIVE_POWER, increase);
    }

    public bool GetTable(int keyCode, out Dictionary<STAT_ON_TABLE, float> table)
    {
        if (keyCode == KeyCode)
        {
            table = mStatTable;

            return true;
        }
        table = null;

        return false;
    }

    public float  MoveSpeed
    {
        get => mStatTable[STAT_ON_TABLE.MOVESPEED];
    }
    public float IMoveSpeed
    {
        get => mStatTable[STAT_ON_TABLE.IMOVESPEED];
        set => mStatTable[STAT_ON_TABLE.IMOVESPEED] = value;
    }
    public float RMoveSpeed
    {
        get => mStatTable[STAT_ON_TABLE.MOVESPEED] + mStatTable[STAT_ON_TABLE.IMOVESPEED];
    }

    public float CurHealth
    {
        get => mStatTable[STAT_ON_TABLE.CURHEALTH];
        set
        {
            float curHealth = mStatTable[STAT_ON_TABLE.CURHEALTH];

            mStatTable[STAT_ON_TABLE.CURHEALTH] = Mathf.Min(curHealth, curHealth + value);
        }
    }
    public float MaxHealth
    {
        get => mStatTable[STAT_ON_TABLE.MAXHEALTH];
    }

    public float  OffensivePower
    {
        get => mStatTable[STAT_ON_TABLE.OFFENSIVE_POWER];
    }
    public float IOffensivePower
    {
        get => mStatTable[STAT_ON_TABLE.IOFFENSIVE_POWER];
        set => mStatTable[STAT_ON_TABLE.IOFFENSIVE_POWER] = value;
    }

    public float ROffensivePower
    {
        get => mStatTable[STAT_ON_TABLE.IOFFENSIVE_POWER] +
               mStatTable[STAT_ON_TABLE.OFFENSIVE_POWER];
    }

    [SerializeField] private float mMoveSpeed;
    [SerializeField] private float mMaxHealth;
    [SerializeField] private float mOffensivePower;
}
