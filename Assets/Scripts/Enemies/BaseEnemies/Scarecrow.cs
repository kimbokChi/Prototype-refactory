using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scarecrow : EnemyBase
{
    [SerializeField]
    private EnemyAnimator EnemyAnimator;

    private Timer mWaitForMove;

    private AttackPeriod mAttackPeriod;

    public override void Damaged(float damage, GameObject attacker)
    {
        if ((AbilityTable.Table[Ability.CurHealth] -= damage) <= 0)
        {
            EnemyAnimator.ChangeState(EnemyAnim.Death);

            HealthBarPool.Instance.UnUsingHealthBar(transform);
        }
    }

    public override void IInit()
    {
        EnemyAnimator?.Init();

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

                EnemyAnimator.ChangeState(EnemyAnim.Move);

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

        if (HasPlayerOnRange() && IsLookAtPlayer())
        {
            mAttackPeriod.StartPeriod();
        }
    }

    protected override void MoveFinish()
    {
        mWaitForMove.Start(WaitMoveTime);

        EnemyAnimator.ChangeState(EnemyAnim.Idle);
    }

    private void AttackAction()
    {
        MoveStop();
        EnemyAnimator.ChangeState(EnemyAnim.Attack);

        if (HasPlayerOnRange() && IsLookAtPlayer()) {
            mPlayer.Damaged(AbilityTable.AttackPower, gameObject);
        }
    }

    private void PlayOverAnimation(EnemyAnim anim)
    {
        switch (anim)
        {
            case EnemyAnim.Attack:
            case EnemyAnim.Damaged:
                EnemyAnimator.ChangeState(EnemyAnim.Idle);
                break;

            case EnemyAnim.Death:
                gameObject.SetActive(false);
                break;
        }
    }
}
