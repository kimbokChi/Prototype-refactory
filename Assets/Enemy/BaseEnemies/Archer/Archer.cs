using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : EnemyBase, IObject, ICombatable
{
    [SerializeField]
    private AbilityTable mAbilityTable;

    [SerializeField]
    private Arrow mArrow;

    [SerializeField]
    private Vector2 mArrowPos;
    [SerializeField]
    private float mArrowSpeed;

    private Timer mWaitForMoving;
    private Timer mWaitForATK;

    private Pool<Arrow> mArrowPool;
    
    private Dictionary<STAT_ON_TABLE, float> mPersonalTable;

    public override AbilityTable GetAbility => mAbilityTable;

    public override void Damaged(float damage, GameObject attacker)
    {
        if ((mPersonalTable[STAT_ON_TABLE.CURHEALTH] -= damage) <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public override void IInit()
    {
        Debug.Assert(mAbilityTable.GetTable(gameObject.GetHashCode(), out mPersonalTable));

        mArrowPool = new Pool<Arrow>();
        mArrowPool.Init(mArrow, Pool_popMethod, Pool_addMethod, Pool_returnToPool);

        mWaitForMoving = new Timer();
        mWaitForATK    = new Timer();

        mWaitForATK.Start(mWaitATKTime);
    }

    public override void IUpdate()
    {
        mArrowPool.Update();

        if (mWaitForMoving.IsOver())
        {
            if (IsMoveFinish && !HasPlayerOnRange())
            {
                Vector2 movePoint;

                movePoint.x = Random.Range(-mHalfMoveRangeX, mHalfMoveRangeX) + mOriginPosition.x;
                movePoint.y = Random.Range(-mHalfMoveRangeY, mHalfMoveRangeY) + mOriginPosition.y;

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

        if (HasPlayerOnRange())
        {
            if (mWaitForATK.IsOver())
            {
                Arrow arrow = mArrowPool.Pop();

                Vector2 targetLocal = PositionLocalized(mPlayer.transform.position);

                arrow.Setting(mArrowSpeed, (targetLocal - (Vector2)transform.localPosition).normalized);
                arrow.Setting(Arrow_targetHit, Arrow_canDistroy);

                mWaitForATK.Start(mWaitATKTime);
            }
            else
            {
                mWaitForATK.Update();
            }
        }
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

    private void Arrow_targetHit(ICombatable combat)
    {
        combat.Damaged(mAbilityTable.RAttackPower, gameObject);
    }

    private bool Arrow_canDistroy(uint hitCount)
    {
        if (hitCount > 0) return true;

        return false;
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
}
