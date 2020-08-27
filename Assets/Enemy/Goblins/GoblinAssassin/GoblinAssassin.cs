using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinAssassin : EnemyBase, IObject, ICombatable
{
    [SerializeField]
    private StatTable mStat;

    [SerializeField]
    private Area mContactArea;

    [SerializeField][Range(0f, 5f)]
    private float mMaxDashLength;

    [SerializeField]
    private float mDashSpeedScale;

    private Timer mWaitForMoving;
    private Timer mWaitForATK;

    private Dictionary<int, bool> mAttackedHashs;

    private Dictionary<STAT_ON_TABLE, float> mStatTable;

    private IEnumerator mEDash;

    public override StatTable Stat => mStat;

    public override void CastBuff(BUFF buffType, IEnumerator castedBuff)
    {
        StartCoroutine(castedBuff);
    }

    public override void Damaged(float damage, GameObject attacker)
    {
        if ((mStatTable[STAT_ON_TABLE.CURHEALTH] -= damage) <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public override void IInit()
    {
        mAttackedHashs = new Dictionary<int, bool>();

        Debug.Assert(mStat.GetTable(gameObject.GetHashCode(), out mStatTable));

        mWaitForMoving = new Timer();
        mWaitForATK    = new Timer();

        mWaitForATK.Start(mWaitATKTime);
    }

    public override bool IsActive() => gameObject.activeSelf;

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
            if (IsMoveFinish && !IsInReachPlayer())
            {
                Vector2 movePoint;

                movePoint.x = Random.Range(-mHalfMoveRangeX, mHalfMoveRangeX) + mOriginPosition.x;
                movePoint.y = Random.Range(-mHalfMoveRangeY, mHalfMoveRangeY) + mOriginPosition.y;

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

    public override GameObject ThisObject() => gameObject;

    private IEnumerator EDash(Vector2 dashPoint)
    {
        dashPoint = FitToMoveArea(dashPoint);

        float lerpAmount = 0;

        Vector2 initPoint = transform.localPosition;

        while (lerpAmount < 1 && Vector2.Distance(initPoint, transform.localPosition) < mMaxDashLength)
        {
            Attack();

            lerpAmount = Mathf.Min(1f, lerpAmount + Time.deltaTime * Time.timeScale * mDashSpeedScale * mStat.RMoveSpeed);

            transform.localPosition = Vector2.Lerp(transform.localPosition, dashPoint, lerpAmount);

            yield return null;
        }
        mAttackedHashs.Clear();

        mWaitForATK.Start(mWaitATKTime); mEDash = null;
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

                combat[0].Damaged(Stat.RAttackPower, gameObject);
            }
        }
    }
}
