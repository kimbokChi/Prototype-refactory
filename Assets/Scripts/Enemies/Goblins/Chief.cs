using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chief : EnemyBase, IObject, ICombatable
{
    private enum MovingDir { Down, Up, Side }

    private enum Pattern
    {
        SummonTotem, SwingRod, SummonBombTotem, END
    }

    public const float  FIRST_STRUGGLE_HP_CONDITION = 0.5f;
    public const float SECOND_STRUGGLE_HP_CONDITION = 0.3f;

    public float HealthPercent
    { get => AbilityTable.Table[Ability.CurHealth] / AbilityTable.Table[Ability.MaxHealth]; }

    private DIRECTION9 mLocation9;

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

    private AttackPeriod mAttackPeriod;

    private bool mHasFirstSTRUGGLE;
    private bool mHasSecondSTRUGGLE;

    private bool mCanCastPATTERN;

    private IEnumerator mESwingRod;

    private event System.Action PatternEnd;

    public override void Damaged(float damage, GameObject attacker)
    {
        AbilityTable.Table[Ability.CurHealth] -= damage;

        if (!mHasFirstSTRUGGLE) {
            if (HealthPercent <= FIRST_STRUGGLE_HP_CONDITION)
            {
                STRUGGLE_summonTotem();

                mHasFirstSTRUGGLE = true;
            }
        }
        if (!mHasSecondSTRUGGLE) {
            if (HealthPercent <= SECOND_STRUGGLE_HP_CONDITION)
            {
                STRUGGLE_summonGoblin();

                mHasSecondSTRUGGLE = true;
            }
        }
        gameObject.SetActive(HealthPercent > 0);
    }

    public override void IInit()
    {
        PatternEnd += PATTERN_moving;

        mAttackPeriod = new AttackPeriod(AbilityTable);
        mAttackPeriod.SetAction(Period.Attack, CastPattern);

        mFloorRooms = Castle.Instance.GetFloorRooms();

        mLocation9 = DIRECTION9.MID;

        mHasFirstSTRUGGLE  = false;
        mHasSecondSTRUGGLE = false;

        mCanCastPATTERN = true;
    }
    public override void IUpdate()
    {
        if (mCanCastPATTERN)
        {
            mAttackPeriod.Update();
        }
    }

    protected override void MoveFinish()
    {
        mCanCastPATTERN = true;
    }

    private void CastPattern()
    {
        mCanCastPATTERN = false;

        switch (GetPATTERN())
        {
            case Pattern.SummonTotem:

                PATTERN_summonTotem();
                break;

            case Pattern.SwingRod:

                PATTERN_swingRod();
                break;

            case Pattern.SummonBombTotem:

                PATTERN_summonBombTotem();
                break;
        }
    }
    private Pattern GetPATTERN()
    {
        Pattern pattern = (Pattern)Random.Range(0, (int)Pattern.END);

        switch (pattern)
        {
            case Pattern.SwingRod:
                if (!HasPlayerOnRange())
                {
                    pattern = (Pattern)Random.Range(1, (int)Pattern.END);
                    
                    if (pattern.Equals(Pattern.SwingRod)) {
                        pattern = Pattern.SummonTotem;
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

        if (instance.TryGetComponent(out IObject Iobject)) parentRoom.AddIObject(Iobject);

        instance.transform.localPosition = 
            mTotemSummonOffset + Vector2.right * Random.Range(-HalfMoveRangeX, HalfMoveRangeX);
    }
    private void SummonLackey(GameObject lackey, Vector2 summonPoint, int roomIndex)
    {
        Room parentRoom = mFloorRooms[roomIndex];

        GameObject instance = Instantiate(lackey, parentRoom.transform, false);

        if (instance.TryGetComponent(out IObject Iobject)) parentRoom.AddIObject(Iobject);

        instance.transform.position = summonPoint;
    }

    private void PATTERN_summonTotem()
    {
        SummonLackey(mTotems[Random.Range(0, mTotems.Length)]);
        PatternEnd.Invoke();
    }
    private void PATTERN_swingRod()
    {
        StartCoroutine(mESwingRod = ESwingRod(3));
    }
    private void PATTERN_summonBombTotem()
    {
        if (mPlayer.TryGetPosition(out Vector2 playerPoint))
        {
            SummonLackey(mBombTotem, playerPoint, (int)mPlayer.GetLPOSITION3());

            PatternEnd.Invoke();
        }
    }
    private void PATTERN_moving()
    {
        if (!IsPlayerLocationAccord())
        {
            if (Random.value < mMovingProbablity) {
                mCanCastPATTERN = true; return;
            }
        }
        DIRECTION9 nextLocation = mLocation9;

        const int MAX = 8;
        const int MIN = 0;

        MovingDir movingDIR = (MovingDir)Random.Range(0, 3);

        switch (movingDIR)
        {
            case MovingDir.Down:
                nextLocation = ((int)(mLocation9 + 3) > MAX) ? mLocation9 : mLocation9 + 3;

                // 아래로 이동할 수 없을 때에는, 위로 이동하도록 한다
                if (nextLocation.Equals(mLocation9))
                {
                    movingDIR = MovingDir.Up;
                    nextLocation = mLocation9 - 3;
                }
                break;

            case MovingDir.Up:
                nextLocation = ((int)(mLocation9 - 3) < MIN) ? mLocation9 : mLocation9 - 3;

                // 위로 이동할 수 없을 때에는, 아래로 이동하도록 한다
                if (nextLocation.Equals(mLocation9))
                {
                    movingDIR = MovingDir.Down;
                    nextLocation = mLocation9 + 3;
                }
                break;
        }
        Vector2 movePoint = Vector2.zero;

        // 위 아래 이동
        if (movingDIR.Equals(MovingDir.Down) || movingDIR.Equals(MovingDir.Up)) 
        {
            movePoint = OriginPosition;

            movePoint.x += transform.localPosition.x;

            movePoint.y += Castle.Instance.GetMovePoint(nextLocation).y + 0.45f;

            mLocation9 = nextLocation;
        }
        // 좌우 이동
        else if (movingDIR.Equals(MovingDir.Side)) 
        {
            movePoint.y += transform.localPosition.y;

            movePoint.x += Random.Range(-HalfMoveRangeX, HalfMoveRangeX);
        }
        switch (movingDIR)
        {
            case MovingDir.Up:
            case MovingDir.Down:
                MoveToPoint(movePoint, MovingStyle.Lerp);
                break;

            case MovingDir.Side:
                MoveToPoint(movePoint, MovingStyle.SmoothDamp);
                break;
        }        
    }
    
    private void STRUGGLE_summonTotem()
    {
        PATTERN_summonTotem();
        PATTERN_summonTotem();
        PATTERN_summonTotem();

        PatternEnd.Invoke();
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

        PatternEnd.Invoke();
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
                mPlayer.Damaged(AbilityTable.AttackPower, gameObject);

            }
        }
        mESwingRod = null; PatternEnd.Invoke();
    }
}