using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chief : EnemyBase, IObject, ICombatable
{
    public const float  FIRST_STRUGGLE_HP_CONDITION = 0.5f;
    public const float SECOND_STRUGGLE_HP_CONDITION = 0.3f;

    public float HealthPercent
    { get => mAbilityTable.Table[Ability.CurHealth] / mAbilityTable.Table[Ability.MaxHealth]; }

    private enum MOVINGDIR
    {
        DOWN, UP, SIDE
    }

    private enum PATTERN
    {
        SUMMON_TOTEM, SWING_ROD, SUMMON_BOMB_TOTEM, END
    }

    private DIRECTION9 mLocation9;

    private PATTERN mCastingPATTERN;

    [Range(0f, 1f)]
    [SerializeField] private float mMovingProbablity;

    [Header("Totems")]
    [SerializeField] private GameObject[]  mTotems;
    [SerializeField] private GameObject mBombTotem;
    [SerializeField] private Vector2 mTotemSummonOffset;

    [Header("Goblins")]
    [SerializeField] private GameObject mGoblinNormal;
    [SerializeField] private GameObject mGoblinDart;
    [SerializeField] private GameObject mGoblinAssassin;

    private Room[] mFloorRooms;

    private Timer mWaitForCastPattern;

    private bool mHasTheFirstSTRUGGLE;
    private bool mHasTheSecondSTRUGGLE;

    private bool mCanCastPATTERN;

    private IEnumerator mESwingRod;

    public override void Damaged(float damage, GameObject attacker)
    {
        if ((mAbilityTable.Table[Ability.CurHealth] -= damage) <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public override void IInit()
    {
        mWaitForCastPattern = new Timer();

        mFloorRooms = Castle.Instance.GetFloorRooms();

        mWaitForCastPattern.Start(mWaitATKTime);

        mLocation9 = DIRECTION9.MID;

        mHasTheFirstSTRUGGLE  = false;
        mHasTheSecondSTRUGGLE = false;

        mCanCastPATTERN = true;

        mCastingPATTERN = GetPATTERN();
    }

    public override bool IsActive()
    {
        return gameObject.activeSelf;
    }

    public override void IUpdate()
    {
        if (!mHasTheFirstSTRUGGLE)
        {
            if (HealthPercent <= FIRST_STRUGGLE_HP_CONDITION)
            {
                STRUGGLE_summonTotem();

                mHasTheFirstSTRUGGLE = true;
            }
        }
        if (!mHasTheSecondSTRUGGLE)
        {
            if (HealthPercent <= SECOND_STRUGGLE_HP_CONDITION)
            {
                STRUGGLE_summonGoblin();

                mHasTheSecondSTRUGGLE = true;
            }
        }        
        if (mWaitForCastPattern.IsOver() && mCanCastPATTERN)
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
            }
            mCanCastPATTERN = false;
        }
        else
        {
            mWaitForCastPattern.Update();
        }
    }

    protected override void MoveFinish()
    {
        mWaitForCastPattern.Start(mWaitATKTime);

        mCanCastPATTERN = true;
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
                if (!HasPlayerOnRange())
                {
                    pattern = (PATTERN)Random.Range(1, (int)PATTERN.END);
                    
                    if (pattern.Equals(PATTERN.SWING_ROD)) {
                        pattern = PATTERN.SUMMON_TOTEM;
                    }
                }
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
    private void SummonLackey(GameObject lackey, Vector2 summonPoint, int roomIndex)
    {
        Room parentRoom = mFloorRooms[roomIndex];

        GameObject instance = Instantiate(lackey, parentRoom.transform, false);

        if (instance.TryGetComponent(out IObject Iobject))
        {
            parentRoom.AddIObject(Iobject);
        }
        instance.transform.position = summonPoint;
    }

    private void PATTERN_summonTotem()
    {
        SummonLackey(mTotems[Random.Range(0, mTotems.Length)]);
        EndOfPattern();
    }
    private void PATTERN_swingRod()
    {
        StartCoroutine(mESwingRod = ESwingRod(3));
    }
    private void PATTERN_summonBombTotem()
    {
        if (mPlayer.Position(out Vector2 playerPoint))
        {
            SummonLackey(mBombTotem, playerPoint, (int)mPlayer.GetLPOSITION3());

            EndOfPattern();
        }
    }
    private void PATTERN_moving()
    {
        if (!IsPlayerLocationAccord())
        {
            if (Random.value < mMovingProbablity) {
                mWaitForCastPattern.Start(mWaitATKTime); mCanCastPATTERN = true;

                return;
            }
        }
        DIRECTION9 nextLocation = mLocation9;

        const int MAX = 8;
        const int MIN = 0;

        MOVINGDIR movingDIR = (MOVINGDIR)Random.Range(0, 3);

        switch (movingDIR)
        {
            case MOVINGDIR.DOWN:
                nextLocation = ((int)(mLocation9 + 3) > MAX) ? mLocation9 : mLocation9 + 3;

                // 아래로 이동할 수 없을 때에는, 위로 이동하도록 한다
                if (nextLocation.Equals(mLocation9))
                {
                    movingDIR = MOVINGDIR.UP;
                    nextLocation = mLocation9 - 3;
                }
                break;

            case MOVINGDIR.UP:
                nextLocation = ((int)(mLocation9 - 3) < MIN) ? mLocation9 : mLocation9 - 3;

                // 위로 이동할 수 없을 때에는, 아래로 이동하도록 한다
                if (nextLocation.Equals(mLocation9))
                {
                    movingDIR = MOVINGDIR.DOWN;
                    nextLocation = mLocation9 + 3;
                }
                break;
        }
        Vector2 movePoint = Vector2.zero;

        // 위 아래 이동
        if (movingDIR.Equals(MOVINGDIR.DOWN) || movingDIR.Equals(MOVINGDIR.UP)) 
        {
            movePoint = mOriginPosition;

            movePoint.x += transform.localPosition.x;

            movePoint.y += Castle.Instance.GetMovePoint(nextLocation).y + 0.45f;

            mLocation9 = nextLocation;
        }
        // 좌우 이동
        else if (movingDIR.Equals(MOVINGDIR.SIDE)) 
        {
            movePoint.y += transform.localPosition.y;

            movePoint.x += Random.Range(-mHalfMoveRangeX, mHalfMoveRangeX);
        }
        switch (movingDIR)
        {
            case MOVINGDIR.UP:
            case MOVINGDIR.DOWN:

                MoveToPoint(movePoint, MOVING_STYLE.Lerp);
                break;

            case MOVINGDIR.SIDE:

                MoveToPoint(movePoint, MOVING_STYLE.SmoothDamp);
                break;
        }        
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
        PATTERN_moving();
    }

    private bool IsPlayerLocationAccord()
    {
        if (mPlayer != null)
        {
            switch (mLocation9)
            {
                case DIRECTION9.TOP_LEFT:
                case DIRECTION9.TOP:
                case DIRECTION9.TOP_RIGHT:
                    return mPlayer.GetLPOSITION3().Equals(LPOSITION3.TOP);

                case DIRECTION9.MID_LEFT:
                case DIRECTION9.MID:
                case DIRECTION9.MID_RIGHT:
                    return mPlayer.GetLPOSITION3().Equals(LPOSITION3.MID);

                case DIRECTION9.BOT_LEFT:
                case DIRECTION9.BOT:
                case DIRECTION9.BOT_RIGHT:
                    return mPlayer.GetLPOSITION3().Equals(LPOSITION3.BOT);
            }
        }
        return false;
    }

    private IEnumerator ESwingRod(int swingCount)
    {
        for (int hit = 0; hit < swingCount; ++hit)
        {
            for (float i = 0; i < 0.15f; i += DeltaTime) { yield return null; }

            if (HasPlayerOnRange())
            {
                mPlayer.Damaged(5f, gameObject);

            }
        }
        mESwingRod = null; EndOfPattern();
    }
}
