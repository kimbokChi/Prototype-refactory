using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin_assassin : EnemyBase, IObject, ICombat
{
    [SerializeField]
    private StatTable mStat;

    private Timer mWaitForMoving;
    private Timer mWaitForATK;

    private Dictionary<STAT_ON_TABLE, float> mStatTable;

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
        if (mWaitForMoving.IsOver())
        {
            if (IsMoveFinish && !IsInReachPlayer())
            {
                Vector2 movePoint;

                movePoint.x = Random.Range(-mHalfMoveRangeX, mHalfMoveRangeX) + mOriginPosition.x;
                movePoint.y = Random.Range(-mHalfMoveRangeY, mHalfMoveRangeY) + mOriginPosition.y;

                if (mPlayer != null)
                {
                    Vector2 lookingDir = movePoint.x > transform.localPosition.x ? Vector2.right : Vector2.left;

                    Vector2 playerPos;

                    if ((IsLookAtPlayer(lookingDir) || IsLookAtPlayer()) && mPlayer.Position(out playerPos))
                    {
                        movePoint = PositionLocalized(playerPos);

                        if (!IsPointOnRange(movePoint))
                        {
                            movePoint -= (movePoint.x > transform.localPosition.x ? Vector2.right : Vector2.left) * mRange;

                        }
                    }
                }
                MoveToPoint(movePoint);
            }
        }
        else
        {
            mWaitForMoving.Update();
        }

        if (IsInReachPlayer())
        {
            if (mWaitForATK.IsOver())
            {

                // To do . . .
            }
            else
            {
                mWaitForATK.Update();
            }
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
}
