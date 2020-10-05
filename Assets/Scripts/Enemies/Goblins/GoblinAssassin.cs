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

    private AttackPeriod mAttackPeriod;
    private Timer mWaitForMoving;

    private IEnumerator mEDash;
    public override void Damaged(float damage, GameObject attacker)
    {
        gameObject.SetActive((AbilityTable.Table[Ability.CurHealth] -= damage) > 0);
    }

    public override void IInit()
    {
        mContactArea.SetEnterAction(Attack);

        mWaitForMoving = new Timer();
         mAttackPeriod = new AttackPeriod(AbilityTable);
         mAttackPeriod.SetAction(Period.Attack, Dash);
    }
    public override void IUpdate()
    {
        if (IsLookAtPlayer() && mEDash == null)
        {
            MoveStop();

            mAttackPeriod.Update();
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

    public override void PlayerEnter(MESSAGE message, Player enterPlayer)
    {
        if (message.Equals(MESSAGE.THIS_ROOM)) mPlayer = enterPlayer;
    }

    public override void PlayerExit(MESSAGE message)
    {
        mPlayer = null;
    }

    private void Dash()
    {
        if (mPlayer.Position(out Vector2 playerPos)) {
            StartCoroutine(mEDash = EDash(PositionLocalized(playerPos)));
        }
    }

    private IEnumerator EDash(Vector2 dashPoint)
    {
        dashPoint = FitToMoveArea(dashPoint).normalized * mMaxDashLength;

        float lerpAmount = 0;

        while (lerpAmount < 1)
        {
            lerpAmount = Mathf.Min(1f, lerpAmount + DeltaTime * mDashSpeedScale * AbilityTable.MoveSpeed);

            transform.localPosition = Vector2.Lerp(transform.localPosition, dashPoint, lerpAmount);

            yield return null;
        }
        mEDash = null;
    }

    private void Attack(GameObject @object)
    {
        if (mEDash != null)
        if (@object.TryGetComponent(out ICombatable combatable))
        {
            Debug.Log(@object.name);
            combatable.Damaged(AbilityTable.AttackPower, gameObject);
        }       
    }
}
