using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Can : EnemyBase
{
    private Timer mWaitForATK;
    private Timer mWaitForMove;

    private Vector2 mOriginPosition;

    private bool mIsMoveFinish;

    public override void Damaged(float damage, GameObject attacker, out GameObject victim)
    {
        victim = gameObject;
    }

    public override void IInit()
    {
        mWaitForATK  = new Timer();
        mWaitForMove = new Timer();

        mOriginPosition = transform.localPosition;

        mIsMoveFinish = true;
    }

    public override void IUpdate()
    {
        if (mWaitForMove.IsOver())
        {
            if (mIsMoveFinish)
            {
                Vector2 movePoint;

                movePoint.x = Random.Range(-mHalfMoveRangeX, mHalfMoveRangeX);
                movePoint.y = Random.Range(-mHalfMoveRangeY, mHalfMoveRangeY) + mOriginPosition.y;

                MoveToPoint(movePoint);

                mIsMoveFinish = false;
            }
        }
        else
        {
            mWaitForMove.Update();
        }
    }

    protected override void MoveFinish()
    {
        mIsMoveFinish = true;
        
        mWaitForMove.Start(mWaitMoveTime);
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

    public override bool IsActive()
    {
        return gameObject.activeSelf;
    }
}
