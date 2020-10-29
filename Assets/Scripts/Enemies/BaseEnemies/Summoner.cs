using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Summoner : EnemyBase
{
    [SerializeField]
    private GameObject mSummonTagret;

    [SerializeField]
    private Vector2 mSummonOffset;

    private Timer mWaitForSummon;
    private Timer mWaitForMove;

    private Room mBelongRoom;

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

        Debug.Assert(transform.parent.TryGetComponent(out mBelongRoom));

        mWaitForSummon = new Timer();
          mWaitForMove = new Timer();

        mWaitForSummon.Start(AbilityTable.BeginAttackDelay);
          mWaitForMove.Start(WaitMoveTime);
    }
    public override void IUpdate()
    {
        if (mWaitForMove.IsOver())
        {
            if (IsMoveFinish)
            {
                Vector2 movePoint;

                movePoint.x = Random.Range(-HalfMoveRangeX, HalfMoveRangeX) + OriginPosition.x;
                movePoint.y = Random.Range(-HalfMoveRangeY, HalfMoveRangeY) + OriginPosition.y;

                MoveToPoint(movePoint);
            }
        }
        else
        {
            mWaitForMove.Update();
        }

        if (mPlayer != null)
        {
            if (mWaitForSummon.IsOver())
            {
                Vector2 summonPoint = mSummonOffset;

                summonPoint.x += Random.Range(-HalfMoveRangeX, HalfMoveRangeX);
                summonPoint.y += Random.Range(-HalfMoveRangeY, HalfMoveRangeY);

                GameObject newObject = Instantiate(mSummonTagret, transform.parent, false);

                if (newObject.TryGetComponent(out IObject @object))
                {
                    mBelongRoom.AddIObject(@object);
                }
                newObject.transform.localPosition = summonPoint;

                mWaitForSummon.Start(AbilityTable.AfterAttackDelay);
            }
            else
            {
                mWaitForSummon.Update();
            }
        }      
    }

    protected override void MoveFinish()
    {
        mWaitForMove.Start(WaitMoveTime);
    }
}
