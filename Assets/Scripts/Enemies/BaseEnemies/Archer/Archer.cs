using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : EnemyBase, IAnimEventReceiver
{
    [SerializeField]
    private Arrow mArrow;

    [SerializeField]
    private Vector2 mArrowPos;
    [SerializeField]
    private float mArrowSpeed;

    [SerializeField]
    private EnemyAnimator EnemyAnimator;

    private Timer mWaitForMoving;
    private Timer mWaitForATK;

    private Pool<Arrow> mArrowPool;

    private AttackPeriod mAttackPeriod;
    
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
        EnemyAnimator.Init();
        HealthBarPool.Instance.UsingHealthBar(-1f, transform, AbilityTable);

        mArrowPool = new Pool<Arrow>();
        mArrowPool.Init(mArrow, Pool_popMethod, Pool_addMethod, Pool_returnToPool);

        mWaitForMoving = new Timer();
        mWaitForATK    = new Timer();

        mAttackPeriod = new AttackPeriod(AbilityTable, 0.583f);

        mAttackPeriod.SetAction(Period.Attack, () => 
        {
            MoveStop();
            EnemyAnimator.ChangeState(AnimState.Attack);
        });
        mAttackPeriod.SetAction(Period.After, AttackAction);

        mWaitForATK.Start(AbilityTable.BeginAttackDelay);
    }

    public override void IUpdate()
    {
        mArrowPool.Update();

        if (mWaitForMoving.IsOver())
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
        else
        {
            mWaitForMoving.Update();
        }

        if (HasPlayerOnRange() && IsLookAtPlayer())
        {
            mAttackPeriod.StartPeriod();            
        }
    }

    protected override void MoveFinish()
    {
        mWaitForMoving.Start(WaitMoveTime);

        EnemyAnimator.ChangeState(AnimState.Idle);
    }

    private void AttackAction()
    {
        Arrow arrow = mArrowPool.Pop();

        Vector2 direction;

        if (SpriteFlipX)
        {
            direction = Vector2.right;
        }
        else
            direction = Vector2.left;

        arrow.Setting(mArrowSpeed, direction);
        arrow.Setting(Arrow_targetHit, i => i > 0);
    }

    private void Arrow_targetHit(ICombatable combat)
    {
        combat.Damaged(AbilityTable.AttackPower, gameObject);
    }
    private void Pool_popMethod(Arrow arrow)
    {
        arrow.transform.position = mArrowPos + (Vector2)transform.position;

        arrow.gameObject.SetActive(true);
    }
    private void Pool_addMethod(Arrow arrow)
    {
        arrow.gameObject.SetActive(false);
    }
    private bool Pool_returnToPool(Arrow arrow)
    {
        return Vector2.Distance(transform.position, arrow.transform.position) > 7f || !arrow.gameObject.activeSelf;
    }

    public void AnimationPlayOver(AnimState anim)
    {
        switch (anim)
        {
            case AnimState.Attack:
            case AnimState.Damaged:
                EnemyAnimator.ChangeState(AnimState.Idle);
                break;

            case AnimState.Death:
                gameObject.SetActive(false);
                break;
        }
    }
}