using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chief : EnemyBase, IObject, ICombatable
{
    private enum MOVINGDIR
    {
        DOWN, UP, SIDE
    }

    private enum PATTERN
    {
        SUMMON_TOTEM, SWING_ROD, SUMMON_BOMB_TOTEM, MOVING, END
    }

    private DIRECTION9 mLocation9;

    private PATTERN mCastingPATTERN;

    [SerializeField] private Vector2 mTotemSummonOffset;
    [SerializeField] private GameObject[]  mTotems;
    [SerializeField] private GameObject mBombTotem;

    [SerializeField] private GameObject mGoblinNormal;
    [SerializeField] private GameObject mGoblinDart;
    [SerializeField] private GameObject mGoblinAssassin;

    private Room[] mFloorRooms;

    private Timer mWaitForCastPattern;

    private Timer mWaitForMove;

    private IEnumerator mESwingRod;

    [SerializeField] private StatTable mStat;

    private Dictionary<STAT_ON_TABLE, float> mStatTable;

    public override StatTable Stat => mStat;

    public override void Damaged(float damage, GameObject attacker)
    {
        if ((mStatTable[STAT_ON_TABLE.CURHEALTH] -= damage) <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public override void IInit()
    {
        Debug.Assert(Stat.GetTable(gameObject.GetHashCode(), out mStatTable));

        mWaitForMove             = new Timer();
        mWaitForCastPattern      = new Timer();

        mFloorRooms = Castle.Instnace.GetFloorRooms();

        mWaitForCastPattern.Start(mWaitATKTime);

        mLocation9 = DIRECTION9.MID;

        mCastingPATTERN = GetPATTERN();
    }

    public override bool IsActive()
    {
        return gameObject.activeSelf;
    }

    public override void IUpdate()
    {
        if (mWaitForMove.IsOver() && IsMoveFinish)
        {
            DIRECTION9 nextLocation = mLocation9;

            const int MAX = 8;
            const int MIN = 0;

            switch ((MOVINGDIR)Random.Range(0, 3))
            {
                case MOVINGDIR.DOWN:
                    nextLocation = ((int)(mLocation9 + 3) > MAX) ? mLocation9 : mLocation9 + 3;
                    break;

                case MOVINGDIR.UP:
                    nextLocation = ((int)(mLocation9 - 3) < MIN) ? mLocation9 : mLocation9 - 3;
                    break;
            }
            Vector2 movePoint = Vector2.zero;

            if (nextLocation != mLocation9) // 위 아래 이동
            {
                movePoint = mOriginPosition;

                movePoint.x += transform.localPosition.x;

                movePoint.y += Castle.Instnace.GetMovePoint(nextLocation).y + 0.45f;

                mLocation9 = nextLocation;
            }
            else // 좌우 이동
            {
                movePoint.y += transform.localPosition.y;

                movePoint.x += Random.Range(-mHalfMoveRangeX, mHalfMoveRangeX);
            }
            MoveToPoint(movePoint);
        }
        else
        {
            mWaitForMove.Update();
        }
        if (mWaitForCastPattern.IsOver())
        {
            mCastingPATTERN = GetPATTERN();

            switch (mCastingPATTERN)
            {
                case PATTERN.SUMMON_TOTEM: 

                    PATTERN_summonTotem();
                    break;

                case PATTERN.SWING_ROD: 

                    PATTERN_swingRod();
                    break;

                case PATTERN.SUMMON_BOMB_TOTEM: 

                    PATTERN_summonBombTotem();
                    break;

                case PATTERN.MOVING:
                    // To do . . .
                    break;

                default:
                    Debug.Log($"{mCastingPATTERN} is undefined");
                    break;
            }
        }
        else
        {
            mWaitForCastPattern.Update();
        }
    }

    protected override void MoveFinish()
    {
        mWaitForMove.Start(WaitMoveTime);
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

    private PATTERN GetPATTERN()
    {
        PATTERN pattern = (PATTERN)Random.Range(0, (int)PATTERN.END);

        switch (pattern)
        {
            case PATTERN.SWING_ROD:
                if (!IsInReachPlayer())
                {
                    pattern = (PATTERN)Random.Range(1, (int)PATTERN.END);
                    
                    if (pattern.Equals(PATTERN.SWING_ROD)) {
                        pattern = PATTERN.SUMMON_TOTEM;
                    }
                }
                break;

            default:
                Debug.Log($"{mCastingPATTERN}s throw condition is undefined");
                break;
        }
        return pattern;
    }
    private void SummonLackey(GameObject lackey)
    {
        Room parentRoom = mFloorRooms[Random.Range(0, mFloorRooms.Length)];

        GameObject instance = Instantiate(lackey, parentRoom.transform, false);

        if (instance.TryGetComponent(out IObject Iobject))
        {
            parentRoom.AddIObject(Iobject);
        }
        Vector2 summonPoint = mTotemSummonOffset;

        summonPoint.x += Random.Range(-mHalfMoveRangeX, mHalfMoveRangeX);

        instance.transform.localPosition = summonPoint;
    }
    private void PATTERN_summonTotem()
    {
        Room parentRoom = mFloorRooms[Random.Range(0, mFloorRooms.Length)];

        GameObject totem = Instantiate(mTotems[Random.Range(0, mTotems.Length)], parentRoom.transform, false);

        if (totem.TryGetComponent(out IObject Iobject))
        {
            parentRoom.AddIObject(Iobject);
        }
        Vector2 summonPoint = mTotemSummonOffset;

        summonPoint.x += Random.Range(-mHalfMoveRangeX, mHalfMoveRangeX);

        totem.transform.localPosition = summonPoint;

        EndOfPattern();
    }
    private void PATTERN_swingRod()
    {
        StartCoroutine(mESwingRod = ESwingRod(3));
    }
    private void PATTERN_summonBombTotem()
    {
        Room parentRoom = mFloorRooms[(int)mPlayer.GetLPOSITION3()];

        GameObject totem = Instantiate(mBombTotem, parentRoom.transform, false);

        if (totem.TryGetComponent(out IObject Iobject))
        {
            parentRoom.AddIObject(Iobject);
        }
        Debug.Assert(mPlayer.Position(out Vector2 playerPoint));

        totem.transform.position = playerPoint;

        EndOfPattern();
    }
    
    private void STRUGGLE_summonTotem()
    {
        PATTERN_summonTotem();
        PATTERN_summonTotem();
        PATTERN_summonTotem();

        EndOfPattern();
    }

    private void STRUGGLE_summonGoblin()
    {
        SummonLackey(mGoblinNormal);
        SummonLackey(mGoblinNormal);
        SummonLackey(mGoblinNormal);
        SummonLackey(mGoblinNormal);

        SummonLackey(mGoblinDart);
        SummonLackey(mGoblinDart);
        
        SummonLackey(mGoblinAssassin);

        EndOfPattern();
    }
    private void EndOfPattern()
    {
        mWaitForCastPattern.Start(mWaitATKTime);
    }

    private IEnumerator ESwingRod(int swingCount)
    {
        for (int hit = 0; hit < swingCount; ++hit)
        {
            for (float i = 0; i < 0.15f; i += Time.deltaTime * Time.timeScale) { yield return null; }

            if (IsInReachPlayer())
            {
                mPlayer.Damaged(5f, gameObject);

                Debug.Log("Continuous-Attack-!");
            }
        }
        mESwingRod = null; EndOfPattern();
    }
}
