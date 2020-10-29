﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scarecrow : EnemyBase
{
    private Timer mWaitForMove;

    private AttackPeriod mAttackPeriod;

    public override void Damaged(float damage, GameObject attacker)
    {
        if ((AbilityTable.Table[Ability.CurHealth] -= damage) <= 0)
        {
            gameObject.SetActive(false);

            HealthBarPool.Instance.UnUsingHealthBar(transform);
        }
    }

    public override void IInit()
    {
        HealthBarPool.Instance.UsingHealthBar(-1f, transform, AbilityTable);

        mWaitForMove = new Timer();

        mAttackPeriod = new AttackPeriod(AbilityTable);
        mAttackPeriod.SetAction(Period.Attack, AttackAction);
    }
    public override void IUpdate()
    {
        if (mWaitForMove.IsOver())
        {
            if (IsMoveFinish && !HasPlayerOnRange())
            {
                Vector2 movePoint;

                movePoint.x = Random.Range(-HalfMoveRangeX, HalfMoveRangeX) + OriginPosition.x;
                movePoint.y = Random.Range(-HalfMoveRangeY, HalfMoveRangeY) + OriginPosition.y;

                if (mPlayer != null)
                {
                    MoveToPlayer(movePoint);
                }
                else MoveToPoint(movePoint);
            }
        }
        else
        {
            mWaitForMove.Update();
        }

        if (HasPlayerOnRange())
        {
            mAttackPeriod.StartPeriod();
        }
    }

    protected override void MoveFinish()
    {
        mWaitForMove.Start(WaitMoveTime);
    }

    private void AttackAction()
    {
        if (HasPlayerOnRange()) {
            mPlayer.Damaged(AbilityTable.AttackPower, gameObject);
        }
    }
}
