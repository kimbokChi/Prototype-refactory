using System.Collections;
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
        EffectLibrary.Instance.UsingEffect(EffectKind.EnemyDmgEffect, transform.position);
        EnemyAnimator.ChangeState(AnimState.Damaged);

        if ((AbilityTable.Table[Ability.CurHealth] -= damage) <= 0)
        {
            AfterImage.SetActive(false);

            mAttackPeriod.StopPeriod();
            EnemyAnimator.ChangeState(AnimState.Death);

            HealthBarPool.Instance.UnUsingHealthBar(transform);
        }
    }

    public override void IInit()
    {
        EnemyAnimator.Init();
        HealthBarPool.Instance.UsingHealthBar(-1f, transform, AbilityTable);

        mContactArea.SetEnterAction(Attack);

        mWaitForMoving = new Timer();

        mAttackPeriod = new AttackPeriod(AbilityTable);

        mAttackPeriod.SetAction(Period.Begin, () => {
            EnemyAnimator.ChangeState(AnimState.AttackBegin);
        });
        mAttackPeriod.SetAction(Period.Attack, AttackAction);
    }
    public override void IUpdate()
    {
        if (!mAttackPeriod.IsProgressing())
        {
            if (IsLookAtPlayer())
            {
                MoveStop();

                mAttackPeriod.StartPeriod();
            }
            else if (mWaitForMoving.IsOver())
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
            else
            {
                mWaitForMoving.Update();
            }
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

        Vector2 force;

        if (SpriteFlipX)
        {
            force = Vector2.right * mMaxDashLength;
        }
        else
            force = Vector2.left * mMaxDashLength;

        StartCoroutine(mEDash = EDash((Vector2)transform.localPosition + force));
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
        dashPoint.x.Range(-HalfMoveRangeX, HalfMoveRangeX);

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
                    AfterImage.transform.parent = transform;

                    mAttackPeriod.AttackActionOver();
                    EnemyAnimator.ChangeState(AnimState.Idle);
                }
                break;
            case AnimState.Damaged:
                {
                    if (IsMoving)
                        EnemyAnimator.ChangeState(AnimState.Move);

                    else
                        EnemyAnimator.ChangeState(AnimState.Idle);
                }
                break;

            case AnimState.Death:
                gameObject.SetActive(false);
                break;
        }
    }
}
