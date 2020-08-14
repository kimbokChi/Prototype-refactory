using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scarecrow : EnemyBase
{
    private Timer mWaitForATK;
    private Timer mWaitForMove;

    public override void Damaged(float damage, GameObject attacker, out GameObject victim)
    {
        victim = gameObject;
    }

    public override void IInit()
    {
        mWaitForATK  = new Timer();
        mWaitForMove = new Timer();
    }

    public override bool IsActive()
    {
        return gameObject.activeSelf;
    }

    public override void IUpdate()
    {
        if (mWaitForMove.IsOver())
        {
            if (IsMoveFinish && !WasLookingAtPlayer())
            {
                Vector2 movePoint;

                movePoint.x = Random.Range(-mHalfMoveRangeX, mHalfMoveRangeX) + mOriginPosition.x;
                movePoint.y = Random.Range(-mHalfMoveRangeY, mHalfMoveRangeY) + mOriginPosition.y;

                if (mPlayer != null)
                {
                    Vector2 lookingDir = movePoint.x > transform.localPosition.x ? Vector2.right : Vector2.left;

                    if (IsLookAtPlayer(lookingDir) || IsLookAtPlayer())
                    {
                        movePoint = PositionLocalized(mPlayer.transform.position);

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
            mWaitForMove.Update();
        }
    }

    protected override void MoveFinish()
    {
        mWaitForMove.Start(WaitMoveTime);
    }

    public override void PlayerEnter(Player enterPlayer)
    {
        mPlayer = enterPlayer;
    }

    public override void PlayerExit()
    {
        mPlayer = null;
    }

    public override GameObject ThisObject()
    {
        return gameObject;
    }

    private bool WasLookingAtPlayer()
    {
        if (mPlayer != null)
        {
            if (IsLookAtPlayer(out Vector2 playerPoint))
            {
                return IsPointOnRange(playerPoint);
            }
        }
        return false;
    }
}
