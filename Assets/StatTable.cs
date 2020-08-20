using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum STAT_ON_TABLE
{
    MOVESPEED, IMOVESPEED,
    CURHEALTH, MAXHEALTH
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

    [SerializeField] private float mMoveSpeed;
    [SerializeField] private float mMaxHealth;
}
