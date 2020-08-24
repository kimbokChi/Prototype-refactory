using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chief : EnemyBase, IObject, ICombat
{
    private Timer mWaitForATK;
    private Timer mWaitForMove;

    [SerializeField] private StatTable mStat;

    private Dictionary<STAT_ON_TABLE, float> mStatTable;

    public override StatTable Stat => mStat;

    public override void Damaged(float damage, GameObject attacker, out GameObject victim)
    {
        victim = gameObject;

        if ((mStatTable[STAT_ON_TABLE.CURHEALTH] -= damage) <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public override void IInit()
    {
        Debug.Assert(Stat.GetTable(gameObject.GetHashCode(), out mStatTable));

        mWaitForATK  = new Timer();
        mWaitForMove = new Timer();

        mWaitForATK.Start(mWaitATKTime);
    }

    public override bool IsActive()
    {
        return gameObject.activeSelf;
    }

    public override void IUpdate()
    {
    }

    public override void PlayerEnter(MESSAGE message, Player enterPlayer)
    {
        mPlayer = enterPlayer;
    }

    public override void PlayerExit(MESSAGE message)
    {
        if (message.Equals(MESSAGE.BELONG_FLOOR))
        {
            mPlayer = null;
        }
    }

    public override void CastBuff(BUFF buffType, IEnumerator castedBuff)
    {
        StartCoroutine(castedBuff);
    }

    public override GameObject ThisObject() => gameObject;
}
