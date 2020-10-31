﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinAssassin : EnemyBase, IAnimEventReceiver
{
    [SerializeField]
    private Area mContactArea;

    [SerializeField][Range(0f, 5f)]
    private float mMaxDashLength;

    [SerializeField]
    private float mDashSpeedScale;

    [SerializeField]
    private GameObject AfterImage;

    [SerializeField]
    private EnemyAnimator EnemyAnimator;

    private AttackPeriod mAttackPeriod;
    private Timer mWaitForMoving;

    private IEnumerator mEDash;
    public override void Damaged(float damage, GameObject attacker)
    {
        gameObject.SetActive((AbilityTable.Table[Ability.CurHealth] -= damage) > 0);
    }

    public override void IInit()
    {
        EnemyAnimator.Init();
        HealthBarPool.Instance.UsingHealthBar(-1f, transform, AbilityTable);

        mContactArea.SetEnterAction(Attack);

        mWaitForMoving = new Timer();

        mAttackPeriod = new AttackPeriod(AbilityTable, 0.25f);

        mAttackPeriod.SetAction(Period.Begin, () => {
            EnemyAnimator.ChangeState(AnimState.AttackBegin);
        });
        mAttackPeriod.SetAction(Period.Attack, AttackAction);
    }
    public override void IUpdate()
    {
        if (IsLookAtPlayer())
        {
            if (mEDash == null)
                MoveStop();

            mAttackPeriod.StartPeriod();
        }
        else if (mWaitForMoving.IsOver())
        {
            if (mEDash == null)
            {
                if (IsMoveFinish && !HasPlayerOnRange())
                {
                    Vector2 movePoint;

                    EnemyAnimator.ChangeState(AnimState.Move);

                    movePoint.x = Random.Range(-HalfMoveRangeX, HalfMoveRangeX) + OriginPosition.x;
                    movePoint.y = Random.Range(-HalfMoveRangeY, HalfMoveRangeY) + OriginPosition.y;

                    MoveToPoint(movePoint);
                }
            }
        }
        else
        {
            mWaitForMoving.Update();
        }
    }
    protected override void MoveFinish()
    {
        EnemyAnimator.ChangeState(AnimState.Idle);

        mWaitForMoving.Start(WaitMoveTime);
    }

    private void AttackAction()
    {
        AfterImage.SetActive(true);
        AfterImage.transform.parent = null;

        AfterImage.transform.position = transform.position;
        EnemyAnimator.ChangeState(AnimState.Attack);

        if (mPlayer) {
            if (mPlayer.TryGetPosition(out Vector2 playerPos))
            {
                StartCoroutine(mEDash = EDash(PositionLocalized(playerPos)));
            }
        }
    }

    private IEnumerator EDash(Vector2 dashPoint)
    {
        Vector2 force;

        if (dashPoint.x - transform.localPosition.x > 0)
        {
            force = Vector2.right * mMaxDashLength;
        }
        else
            force = Vector2.left * mMaxDashLength;

        dashPoint = (Vector2)transform.localPosition + force;

        float lerpAmount = 0;

        while (lerpAmount < 1)
        {
            lerpAmount = Mathf.Min(1f, lerpAmount + DeltaTime * mDashSpeedScale * AbilityTable.MoveSpeed);

            transform.localPosition = Vector3.Lerp(transform.localPosition, dashPoint, lerpAmount);

            yield return null;
        }
        mEDash = null;
    }

    private void Attack(GameObject @object)
    {
        if (mEDash != null)
        if (@object.TryGetComponent(out ICombatable combatable))
        {
            combatable.Damaged(AbilityTable.AttackPower, gameObject);
        }
    }

    public void AnimationPlayOver(AnimState anim)
    {
        switch (anim)
        {
            case AnimState.Attack:
                {
                    AfterImage.SetActive(false);
                    AfterImage.transform.parent = transform;

                    EnemyAnimator.ChangeState(AnimState.Idle);
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
