using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Castle : Singleton<Castle>
{
    private bool mIsActivation = true;

    private Player mEyePlayer;

    private LPOSITION3 mLastPlayerPOS = LPOSITION3.NONE;

    [SerializeField] private   Floor[] mFloors;
    [SerializeField] private   Floor   mCurrentFloor;
                     private Vector2[] mMovePoints;


    public Vector2 GetMovePoint(DIRECTION9 direction)
    {
        return mMovePoints[(int)direction];
    }

    public Vector2 NextFloor()
    {
        int playerPOS = (int)mEyePlayer.GetTPOSITION3();

        Floor moveFloor;

        // 더이상 위 지역으로 이동할 수 없을때
        if (IsIndexOutFloor(mCurrentFloor.PairOfStairs))
        {
            moveFloor = mFloors[mCurrentFloor.PairOfStairs - 1];

            return moveFloor.GetMovePoints(LPOSITION3.TOP)[playerPOS];
        }
        else
        {
            moveFloor = mFloors[mCurrentFloor.PairOfStairs];

            return moveFloor.GetMovePoints(LPOSITION3.BOT)[playerPOS];
        }
    }
    public void AliveNextFloor()
    {
        // 위 지역으로 이동할 수 있을때
        if (!IsIndexOutFloor(mCurrentFloor.PairOfStairs)) 
        {
            mCurrentFloor = mFloors[mCurrentFloor.PairOfStairs];

            Renew();
        }
    }

    private bool IsIndexOutFloor(int floorNumber)
    {
        return (mFloors.Length <= floorNumber);
    }

    private void BuildCastle()
    {
        for (int i = 0; i < mFloors.Length; ++i)
        {
            mFloors[i].BuildRoom();
        }
    }

    private void PlayerRegister(int floor)
    {
        if (mFloors[floor - 1] != null)
        {
            mCurrentFloor = mFloors[floor - 1];

            mEyePlayer = FindObjectOfType(typeof(Player)) as Player;
        }
    }

    private void Renew()
    {
        mCurrentFloor.IInit();

        if (mEyePlayer)
        {
            RenewPlayerPOS();
        }
        Vector2[] topMovePoint = mCurrentFloor.GetMovePoints(LPOSITION3.TOP);
        Vector2[] midMovePoint = mCurrentFloor.GetMovePoints(LPOSITION3.MID);
        Vector2[] botMovePoint = mCurrentFloor.GetMovePoints(LPOSITION3.BOT);

        mMovePoints = new Vector2[(int)DIRECTION9.END]
        {
            topMovePoint[0], topMovePoint[1], topMovePoint[2],
            midMovePoint[0], midMovePoint[1], midMovePoint[2],
            botMovePoint[0], botMovePoint[1], botMovePoint[2]
        };
    }
    private void RenewPlayerPOS()
    {
        if (mLastPlayerPOS != mEyePlayer.GetLPOSITION3())
        {
            if (mLastPlayerPOS != LPOSITION3.NONE)
            {
                mCurrentFloor.ExitPlayer(mLastPlayerPOS);
            }
            mCurrentFloor.EnterPlayer(mLastPlayerPOS = mEyePlayer.GetLPOSITION3());
        }
    }

    private IEnumerator CR_update()
    {
        while (mIsActivation)
        {
            if (mEyePlayer)
            {
                RenewPlayerPOS();
            }
            if (mCurrentFloor)
            {
                mCurrentFloor.IUpdate();
            }
            yield return null;
        }
        yield break;
    }

    private void Start()
    {
        BuildCastle();

        PlayerRegister(1);

        Renew();

        StartCoroutine(CR_update());
    }
}
