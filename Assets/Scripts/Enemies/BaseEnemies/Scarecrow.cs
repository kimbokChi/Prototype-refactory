using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scarecrow : EnemyBase, IAnimEventReceiver
{
    [SerializeField]
    private EnemyAnimator EnemyAnimator;

    [SerializeField]
    private float AttackTime;

    private Timer mWaitForMove;

    private AttackPeriod mAttackPeriod;

    private bool mCanMoving;

    public override void Damaged(float damage, GameObject attacker)
    {
        EnemyAnimator.ChangeState(AnimState.Damaged);

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

        mAttackPeriod = new AttackPeriod(AbilityTable, AttackTime);
        mAttackPeriod.SetAction(Period.Attack,() =>
        {
            MoveStop(); mCanMoving = false;
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
        if (HasPlayerOnRange() && IsLookAtPlayer()) {
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
