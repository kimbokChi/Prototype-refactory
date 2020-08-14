using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Can : EnemyBase
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
