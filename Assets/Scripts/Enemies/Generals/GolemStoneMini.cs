using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemStoneMini : EnemyBase, IAnimEventReceiver
{
    [SerializeField]
    private EnemyAnimator EnemyAnimator;

    private Timer mWaitForMove;

    private AttackPeriod mAttackPeriod;

    private bool mCanMoving;

    public override void Damaged(float damage, GameObject attacker)
    {
        EffectLibrary.Instance.UsingEffect(EffectKind.EnemyDmgEffect, transform.position);

        if ((AbilityTable.Table[Ability.CurHealth] -= damage) <= 0)
        {
            EnemyAnimator.ChangeState(AnimState.Death);

            HealthBarPool.Instance.UnUsingHealthBar(transform);
        }
    }

    public override void IInit()
    {
        EnemyAnimator?.Init();

        HealthBarPool.Instance.UsingHealthBar(-1f, transform, AbilityTable);

        mWaitForMove = new Timer();

        mAttackPeriod = new AttackPeriod(AbilityTable, 0.417f);

        mAttackPeriod.SetAction(Period.Begin, () => {
            mCanMoving = false;
            MoveStop();
            EnemyAnimator.ChangeState(AnimState.AttackBegin);
        });
        mAttackPeriod.SetAction(Period.Attack, () => {
            EnemyAnimator.ChangeState(AnimState.Attack);
        });
        mCanMoving = true;
    }
    public override void IUpdate()
    {
        if (mWaitForMove.IsOver())
        {
            if (mCanMoving)
            {

                if (IsMoveFinish && !HasPlayerOnRange())
                {
                    Vector2 movePoint;

                    movePoint.x = Random.Range(-HalfMoveRangeX, HalfMoveRangeX) + OriginPosition.x;
                    movePoint.y = Random.Range(-HalfMoveRangeY, HalfMoveRangeY) + OriginPosition.y;

                    EnemyAnimator.ChangeState(AnimState.Move);

                    if (mPlayer != null)
                    {
                        MoveToPlayer(movePoint);
                    }
                    else MoveToPoint(movePoint);
                }
            }
        }
        else
        {
            mWaitForMove.Update();
        }

        if (HasPlayerOnRange() && IsLookAtPlayer())
        {
            mAttackPeriod.StartPeriod();
        }
    }

    protected override void MoveFinish()
    {
        mWaitForMove.Start(WaitMoveTime);

        EnemyAnimator.ChangeState(AnimState.Idle);
    }

    private void AttackAction()
    {
        if (HasPlayerOnRange() && IsLookAtPlayer())
        {
            mPlayer.Damaged(AbilityTable.AttackPower, gameObject);
        }
    }

    public void AnimationPlayOver(AnimState anim)
    {
        switch (anim)
        {
            case AnimState.Attack:
                {
                    EnemyAnimator.ChangeState(AnimState.Idle);
                    mCanMoving = true;
                }
                break;
            case AnimState.Damaged:
                EnemyAnimator.ChangeState(AnimState.Idle);
                break;

            case AnimState.Death:
                gameObject.SetActive(false);
                break;
        }
    }
}
