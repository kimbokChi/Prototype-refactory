using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Summoner : EnemyBase, IObject, ICombatable
{
    [SerializeField]
    private GameObject mSummonTagret;

    [SerializeField]
    private Vector2 mSummonOffset;

    [SerializeField]
    private float mWaitSummon;
    private Timer mWaitForSummon;
    private Timer mWaitForMove;

    private Room mBelongRoom;

    public override void Damaged(float damage, GameObject attacker)
    {
        if ((AbilityTable.Table[Ability.CurHealth] -= damage) <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public override void IInit()
    {
        Debug.Assert(transform.parent.TryGetComponent(out mBelongRoom));

        mWaitForSummon = new Timer();
          mWaitForMove = new Timer();

        mWaitForSummon.Start(mWaitSummon);
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

                mWaitForSummon.Start(mWaitSummon);
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

    public override void PlayerEnter(MESSAGE message, Player enterPlayer)
    {
        if (message.Equals(MESSAGE.THIS_ROOM))
        {
            mPlayer = enterPlayer;
        }
    }
    public override void PlayerExit(MESSAGE message)
    {
        mPlayer = null;
    }

    public override void CastBuff(BUFF buffType, IEnumerator castedBuff)
    {
        StartCoroutine(castedBuff);
    }
    public override bool IsActive()
    {
        return gameObject.activeSelf;
    }
    public override GameObject ThisObject() => gameObject;
}
