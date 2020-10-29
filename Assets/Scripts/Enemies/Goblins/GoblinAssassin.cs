using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinAssassin : EnemyBase, IObject, ICombatable
{
    [SerializeField]
    private Area mContactArea;

    [SerializeField][Range(0f, 5f)]
    private float mMaxDashLength;

    [SerializeField]
    private float mDashSpeedScale;

    [SerializeField]
    private GameObject AfterImage;

    private AttackPeriod mAttackPeriod;
    private Timer mWaitForMoving;

    private IEnumerator mEDash;
    public override void Damaged(float damage, GameObject attacker)
    {
        gameObject.SetActive((AbilityTable.Table[Ability.CurHealth] -= damage) > 0);
    }

    public override void IInit()
    {
        HealthBarPool.Instance.UsingHealthBar(-1f, transform, AbilityTable);

        mContactArea.SetEnterAction(Attack);

        mWaitForMoving = new Timer();
         mAttackPeriod = new AttackPeriod(AbilityTable);
         mAttackPeriod.SetAction(Period.Attack, Dash);
    }
    public override void IUpdate()
    {
        if (IsLookAtPlayer())
        {
            if (mEDash == null) MoveStop();

            mAttackPeriod.StartPeriod();
        }
        else if (mWaitForMoving.IsOver())
        {
            if (IsMoveFinish && !HasPlayerOnRange())
            {
                Vector2 movePoint;

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

    private void Dash()
    {
        if (mPlayer)
        if (mPlayer.TryGetPosition(out Vector2 playerPos)) {
            StartCoroutine(mEDash = EDash(PositionLocalized(playerPos)));
        }
    }

    private IEnumerator EDash(Vector2 dashPoint)
    {
        AfterImage.SetActive(true);

        dashPoint = (Vector2)transform.localPosition + 
             ((dashPoint.x - transform.localPosition.x > 0) ? Vector2.right : Vector2.left) * mMaxDashLength;

        float lerpAmount = 0;

        while (lerpAmount < 1)
        {
            lerpAmount = Mathf.Min(1f, lerpAmount + DeltaTime * mDashSpeedScale * AbilityTable.MoveSpeed);

            transform.localPosition = Vector3.Lerp(transform.localPosition, dashPoint, lerpAmount);

            yield return null;
        }
        mEDash = null;

        AfterImage.SetActive(false);
    }

    private void Attack(GameObject @object)
    {
        if (mEDash != null)
        if (@object.TryGetComponent(out ICombatable combatable))
        {
                combatable.Damaged(AbilityTable.AttackPower, gameObject);
        }       
    }
}
