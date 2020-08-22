using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Summoner : EnemyBase, IObject, ICombat
{
    [SerializeField]
    private StatTable mStat;

    [SerializeField]
    private GameObject mSummonTagret;

    [SerializeField]
    private Vector2 mSummonOffset;

    [SerializeField]
    private float mWaitSummon;
    private Timer mWaitForSummon;
    private Timer mWaitForMove;

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
        Debug.Assert(mStat.GetTable(gameObject.GetHashCode(), out mStatTable));

        mWaitForSummon = new Timer();
          mWaitForMove = new Timer();

        mWaitForSummon.Start(mWaitSummon);
          mWaitForMove.Start(WaitMoveTime);
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

        if (mPlayer != null)
        {
            if (mWaitForSummon.IsOver())
            {
                Vector2 summonPoint = mSummonOffset;

                summonPoint.x += Random.Range(-mHalfMoveRangeX, mHalfMoveRangeX);
                summonPoint.y += Random.Range(-mHalfMoveRangeY, mHalfMoveRangeY);

                Instantiate(mSummonTagret, summonPoint, Quaternion.identity);

                mWaitForSummon.Start(mWaitSummon);
            }
            else
            {
                mWaitForSummon.Update();
            }
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

    public override void CastBuff(BUFF buffType, IEnumerator castedBuff)
    {
        StartCoroutine(castedBuff);
    }
    public override bool IsActive()
    {
        return gameObject.activeSelf;
    }
    public override GameObject ThisObject() => gameObject;
}
