using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum STAT_ON_TABLE
{
    MOVESPEED, IMOVESPEED
}

public class StatTable : MonoBehaviour
{
    private int KeyCode;
    private Dictionary<STAT_ON_TABLE, float> mStatTable;

    [SerializeField]
    private GameObject BelongObject;

    private void Awake()
    {
        KeyCode = BelongObject.GetHashCode();

        mStatTable = new Dictionary<STAT_ON_TABLE, float>();

        mStatTable.Add(STAT_ON_TABLE. MOVESPEED,  mMoveSpeed);
        mStatTable.Add(STAT_ON_TABLE.IMOVESPEED, mIMoveSpeed);
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

        set => mIMoveSpeed = value - MoveSpeed;
    }
    public float IMoveSpeed
    {
        get { return mStatTable[STAT_ON_TABLE.IMOVESPEED]; }
    }

    [SerializeField] private float  mMoveSpeed;
    [SerializeField] private float mIMoveSpeed;
}
