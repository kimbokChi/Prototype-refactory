﻿using System.Collections;
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

    private Timer mWaitForMoving;
    private Timer mWaitForATK;

    private Dictionary<int, bool> mAttackedHashs;

    private IEnumerator mEDash;
    public override void Damaged(float damage, GameObject attacker)
    {
        if ((AbilityTable.Table[Ability.CurHealth] -= damage) <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public override void IInit()
    {
        mAttackedHashs = new Dictionary<int, bool>();

        mWaitForMoving = new Timer();
        mWaitForATK    = new Timer();

        mWaitForATK.Start(AbilityTable.BeginAttackDelay);
    }
    public override void IUpdate()
    {
        if (IsLookAtPlayer())
        {
            if (mEDash == null)
            {
                MoveStop();

                if (mWaitForATK.IsOver())
                {
                    if (mPlayer.Position(out Vector2 playerPos))
                    {
                        playerPos = PositionLocalized(playerPos);
                        
                        StartCoroutine(mEDash = EDash(playerPos));
                    }
                }
                else
                {
                    mWaitForATK.Update();
                }
            }
        }
        else if (mWaitForMoving.IsOver())
        {
            if (IsMoveFinish && !HasPlayerOnRange())
            {
                Vector2 movePoint;

                movePoint.x = Random.Range(-HalfMoveRangeX, HalfMoveRangeX) + OriginPosition.x;
                movePoint.y = Random.Range(-HalfMoveRangeY, HalfMoveRangeY) + OriginPosition.y;

                // Cut

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
    private IEnumerator EDash(Vector2 dashPoint)
    {
        dashPoint = FitToMoveArea(dashPoint);

        float lerpAmount = 0;

        Vector2 initPoint = transform.localPosition;

        while (lerpAmount < 1 && Vector2.Distance(initPoint, transform.localPosition) < mMaxDashLength)
        {
            Attack();

            lerpAmount = Mathf.Min(1f, lerpAmount + DeltaTime * mDashSpeedScale * AbilityTable.MoveSpeed);

            transform.localPosition = Vector2.Lerp(transform.localPosition, dashPoint, lerpAmount);

            yield return null;
        }
        mAttackedHashs.Clear();

        mWaitForATK.Start(AbilityTable.AfterAttackDelay); mEDash = null;
    }

    private void Attack()
    {
        ICombatable[] combat = mContactArea.GetEnterTypeT<ICombatable>();

        for (int i = 0; i < combat.Length; ++i)
        {
            int hash = combat[i].GetHashCode();

            if (!mAttackedHashs.ContainsKey(hash))
            {
                mAttackedHashs.Add(hash, true);

                combat[0].Damaged(AbilityTable.AttackPower, gameObject);
            }
        }
    }
}
