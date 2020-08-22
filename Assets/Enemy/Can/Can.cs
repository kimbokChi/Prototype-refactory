using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Can : EnemyBase
{
    private Timer mWaitForATK;
    private Timer mWaitForMove;

    private Dictionary<STAT_ON_TABLE, float> mStatTable;

    [SerializeField] private StatTable mStat;

    public override StatTable Stat => mStat;

    public override void Damaged(float damage, GameObject attacker, out GameObject victim)
    {
        victim = gameObject;

        mStatTable[STAT_ON_TABLE.CURHEALTH] -= damage;

        if (mStatTable[STAT_ON_TABLE.CURHEALTH] <= 0) gameObject.SetActive(false);
    }

    public override void IInit()
    {
        mWaitForATK  = new Timer();
        mWaitForMove = new Timer();

        Debug.Assert(mStat.GetTable(gameObject.GetHashCode(), out mStatTable));
    }

    public override void IUpdate()
    {
        if (mWaitForMove.IsOver())
        {
            if (IsMoveFinish)
            {
                Vector2 movePoint;

                movePoint.x = Random.Range(-mHalfMoveRangeX, mHalfMoveRangeX) + mOriginPosition.x;
                movePoint.y = Random.Range(-mHalfMoveRangeY, mHalfMoveRangeY) + mOriginPosition.y;

                MoveToPoint(movePoint);
            }
        }
        else
        {
            mWaitForMove.Update();
        }
    }

    protected override void MoveFinish()
    {        
        mWaitForMove.Start(WaitMoveTime);
    }

    public override void PlayerEnter(MESSAGE message, Player enterPlayer)
    {
        if (message.Equals(MESSAGE.THIS_ROOM))
        {
            mPlayer = enterPlayer;
        }
    }

    public override void PlayerExit(MESSAGE message)
    {
        mPlayer = null;
    }

    public override GameObject ThisObject()
    {
        return gameObject;
    }

    public override bool IsActive()
    {
        return gameObject.activeSelf;
    }

    public override void CastBuff(BUFF buffType, IEnumerator castedBuff)
    {
        StartCoroutine(castedBuff);
    }
}
