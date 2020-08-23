using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : EnemyBase, IObject, ICombat
{
    [SerializeField]
    private StatTable mStat;

    [SerializeField]
    private Arrow mArrow;

    [SerializeField]
    private Vector2 mArrowPos;
    [SerializeField]
    private float mArrowSpeed;

    private Timer mWaitForMoving;
    private Timer mWaitForATK;
    
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

        mWaitForMoving = new Timer();
        mWaitForATK    = new Timer();

        mWaitForATK.Start(mWaitATKTime);
    }

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
                Arrow arrow = Instantiate(mArrow, mArrowPos + (Vector2)transform.position, Quaternion.identity);

                Vector2 targetLocal = PositionLocalized(mPlayer.transform.position);

                arrow.Setting(mArrowSpeed, (targetLocal - (Vector2)transform.localPosition).normalized);
                arrow.Setting(Arrow_targetHit, Arrow_canDistroy);

                mWaitForATK.Start(mWaitATKTime);
            }
            else
            {
                mWaitForATK.Update();
            }
        }
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

    private void Arrow_targetHit(ICombat combat)
    {
        combat.Damaged(mStat.RAttackPower, gameObject, out GameObject v);
    }

    private bool Arrow_canDistroy(uint hitCount)
    {
        if (hitCount > 0) return true;

        return false;
    }
}
