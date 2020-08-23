using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin_assassin : EnemyBase, IObject, ICombat
{
    [SerializeField]
    private StatTable mStat;

    [SerializeField][Range(0f, 5f)]
    private float mMaxDashLength;

    private Timer mWaitForMoving;
    private Timer mWaitForATK;

    private Dictionary<STAT_ON_TABLE, float> mStatTable;

    private IEnumerator mEDash;

    public override StatTable Stat => mStat;

    public override void CastBuff(BUFF buffType, IEnumerator castedBuff)
    {
        StartCoroutine(castedBuff);
    }

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

        mWaitForMoving = new Timer();
        mWaitForATK    = new Timer();

        mWaitForATK.Start(mWaitATKTime);
    }

    public override bool IsActive() => gameObject.activeSelf;

    public override void IUpdate()
    {
        if (IsLookAtPlayer())
        {
            if (mEDash == null)
            {
                MoveStop();

                if (mWaitForATK.IsOver())
                {
                    if (mPlayer.Position(out Vector2 playerPos))
                    {
                        playerPos = PositionLocalized(playerPos);
                        
                        StartCoroutine(mEDash = EDash(playerPos, 1.8f));
                    }
                }
                else
                {
                    mWaitForATK.Update();
                }
            }
        }
        else if (mWaitForMoving.IsOver())
        {
            if (IsMoveFinish && !IsInReachPlayer())
            {
                Vector2 movePoint;

                movePoint.x = Random.Range(-mHalfMoveRangeX, mHalfMoveRangeX) + mOriginPosition.x;
                movePoint.y = Random.Range(-mHalfMoveRangeY, mHalfMoveRangeY) + mOriginPosition.y;

                // Cut

                MoveToPoint(movePoint);
            }
        }
        else
        {
            mWaitForMoving.Update();
        }
    }

    public override void PlayerEnter(MESSAGE message, Player enterPlayer)
    {
        if (message.Equals(MESSAGE.THIS_ROOM)) mPlayer = enterPlayer;
    }

    public override void PlayerExit(MESSAGE message)
    {
        mPlayer = null;
    }

    public override GameObject ThisObject() => gameObject;

    private IEnumerator EDash(Vector2 dashPoint, float accel = 1)
    {
        dashPoint = FitToMoveArea(dashPoint);

        float lerpAmount = 0;

        Vector2 initPoint = transform.localPosition;

        while (lerpAmount < 1 && Vector2.Distance(initPoint, transform.localPosition) < mMaxDashLength)
        {
            lerpAmount = Mathf.Min(1f, lerpAmount + Time.deltaTime * Time.timeScale * accel * mStat.RMoveSpeed);

            transform.localPosition = Vector2.Lerp(transform.localPosition, dashPoint, lerpAmount);

            yield return null;
        }
        mWaitForATK.Start(mWaitATKTime); mEDash = null;
    }
}
