using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chief : EnemyBase, IObject, ICombat
{
    private enum MOVINGDIR
    {
        DOWN, UP, SIDE
    }

    private DIRECTION9 mLocation9;

    [SerializeField] private float mWaitSummonTotem;
    [SerializeField] private float mWaitContinuousAttack;

    [SerializeField] private Vector2 mTotemSummonOffset;
    [SerializeField] private GameObject[] mTotems;

    private Room[] mFloorRooms;

    private Timer mWaitForSummonTotem;
    private Timer mWaitForContinuousAttack;

    private Timer mWaitForMove;

    private IEnumerator mEContinuousAttack;

    [SerializeField] private StatTable mStat;

    private Dictionary<STAT_ON_TABLE, float> mStatTable;

    public override StatTable Stat => mStat;

    public override void Damaged(float damage, GameObject attacker, out GameObject victim)
    {
        victim = gameObject;

        if ((mStatTable[STAT_ON_TABLE.CURHEALTH] -= damage) <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public override void IInit()
    {
        Debug.Assert(Stat.GetTable(gameObject.GetHashCode(), out mStatTable));

        mWaitForSummonTotem      = new Timer();
        mWaitForContinuousAttack = new Timer();
        mWaitForMove             = new Timer();

        mFloorRooms = Castle.Instnace.GetFloorRooms();

             mWaitForSummonTotem.Start(mWaitSummonTotem);
        mWaitForContinuousAttack.Start(mWaitContinuousAttack);
    }

    public override bool IsActive()
    {
        return gameObject.activeSelf;
    }

    public override void IUpdate()
    {
        if (mWaitForMove.IsOver())
        {
            DIRECTION9 nextLocation;

            const int MAX = 9;
            const int MIN = 0;

            switch ((MOVINGDIR)Random.Range(0, 3))
            {
                case MOVINGDIR.DOWN:
                    nextLocation = ((int)mLocation9 + 3 > MAX) ? mLocation9 : mLocation9 + 3;
                    break;

                case MOVINGDIR.UP:
                    nextLocation = ((int)mLocation9 - 3 < MIN) ? mLocation9 : mLocation9 - 3;
                    break;

                case MOVINGDIR.SIDE:
                    // To do . . .
                    break;
            }
        }

        if (mWaitForSummonTotem.IsOver())
        {
            Skill_summonTotem();

            mWaitForSummonTotem.Start(mWaitSummonTotem);
        }
        else
        {
            mWaitForSummonTotem.Update();
        }

        if (mWaitForContinuousAttack.IsOver())
        {
            if (mPlayer != null)
            {
                if (IsInReachPlayer() && mEContinuousAttack == null)
                {
                    StartCoroutine(mEContinuousAttack = EContinuousAttack(8));

                }
            }
        }
        else
        {
            mWaitForContinuousAttack.Update();
        }
    }

    public override void PlayerEnter(MESSAGE message, Player enterPlayer)
    {
        mPlayer = enterPlayer;
    }

    public override void PlayerExit(MESSAGE message)
    {
        if (message.Equals(MESSAGE.BELONG_FLOOR))
        {
            mPlayer = null;
        }
    }

    public override void CastBuff(BUFF buffType, IEnumerator castedBuff)
    {
        StartCoroutine(castedBuff);
    }

    public override GameObject ThisObject() => gameObject;

    private void Skill_summonTotem(int summonCount = 2)
    {
        for (int i = 0; i < summonCount; i++)
        {
            Room parentRoom = mFloorRooms[Random.Range(0, mFloorRooms.Length)];

            GameObject totem = Instantiate(mTotems[Random.Range(0, mTotems.Length)], parentRoom.transform, false);

            if (totem.TryGetComponent(out IObject Iobject))
            {
                parentRoom.AddIObject(Iobject);
            }
            Vector2 summonPoint = mTotemSummonOffset;

            summonPoint.x += Random.Range(-mHalfMoveRangeX, mHalfMoveRangeX);
            summonPoint.y += Random.Range(-mHalfMoveRangeY, mHalfMoveRangeY);

            totem.transform.localPosition = summonPoint;
        }
    }

    private IEnumerator EContinuousAttack(int hitCount)
    {
        for (int hit = 0; hit < hitCount; ++hit)
        {
            for (float i = 0; i < 0.15f; i += Time.deltaTime * Time.timeScale) { yield return null; }

            if (IsInReachPlayer())
            {
                mPlayer.Damaged(5f, gameObject, out GameObject v);

                Debug.Log("Continuous-Attack-!");
            }
        }
        mEContinuousAttack = null;

        mWaitForContinuousAttack.Start(mWaitContinuousAttack);
    }
}
