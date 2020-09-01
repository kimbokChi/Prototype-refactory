using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum STAT_ON_TABLE
{
    MOVESPEED, IMOVESPEED,
    CURHEALTH, MAXHEALTH,
    ATTACK_POWER, IATTACK_POWER
}

public class AbilityTable : MonoBehaviour
{
    private int KeyCode;
    private Dictionary<STAT_ON_TABLE, float> mPersonalTable;

    [SerializeField] private GameObject BelongObject;

    private void Reset() {
        BelongObject = transform.parent.gameObject;
    }

    private void Awake()
    {
        KeyCode = BelongObject.GetHashCode();

        mPersonalTable = new Dictionary<STAT_ON_TABLE, float>();

        float increase = 0f;

        mPersonalTable.Add(STAT_ON_TABLE. MOVESPEED, mMoveSpeed);
        mPersonalTable.Add(STAT_ON_TABLE.IMOVESPEED, increase);

        mPersonalTable.Add(STAT_ON_TABLE.CURHEALTH, mMaxHealth);
        mPersonalTable.Add(STAT_ON_TABLE.MAXHEALTH, mMaxHealth);

        mPersonalTable.Add(STAT_ON_TABLE. ATTACK_POWER, mAttackPower);
        mPersonalTable.Add(STAT_ON_TABLE.IATTACK_POWER, increase);
    }

    public bool GetTable(int keyCode, out Dictionary<STAT_ON_TABLE, float> table)
    {
        if (keyCode == KeyCode) {
            table = mPersonalTable; return true;
        }
        table = null;

        return false;
    }

    // MoveSpeed
    public float  MoveSpeed
    {
        get => mPersonalTable[STAT_ON_TABLE.MOVESPEED];
    }
    public float IMoveSpeed
    {
        get => mPersonalTable[STAT_ON_TABLE.IMOVESPEED];
        set => mPersonalTable[STAT_ON_TABLE.IMOVESPEED] = value;
    }
    public float RMoveSpeed
    {
        get => mPersonalTable[STAT_ON_TABLE.MOVESPEED] + mPersonalTable[STAT_ON_TABLE.IMOVESPEED];
    }

    // Health
    public float CurHealth
    {
        get => mPersonalTable[STAT_ON_TABLE.CURHEALTH];
        set
        {
            float curHealth = mPersonalTable[STAT_ON_TABLE.CURHEALTH];

            mPersonalTable[STAT_ON_TABLE.CURHEALTH] = Mathf.Min(curHealth, curHealth + value);
        }
    }
    public float MaxHealth
    {
        get => mPersonalTable[STAT_ON_TABLE.MAXHEALTH];
    }

    // AttackPower
    public float  AttackPower
    {
        get => mPersonalTable[STAT_ON_TABLE.ATTACK_POWER];
    }
    public float IAttackPower
    {
        get => mPersonalTable[STAT_ON_TABLE.IATTACK_POWER];
        set => mPersonalTable[STAT_ON_TABLE.IATTACK_POWER] = value;
    }
    public float RAttackPower
    {
        get => mPersonalTable[STAT_ON_TABLE.IATTACK_POWER] +
               mPersonalTable[STAT_ON_TABLE.ATTACK_POWER];
    }

    [SerializeField] private float mMoveSpeed;
    [SerializeField] private float mMaxHealth;
    [SerializeField] private float mAttackPower;
}
