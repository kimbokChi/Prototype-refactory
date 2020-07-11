using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Castle : Singleton<Castle>
{
    private bool mIsActivation = true;

    private Player mEyePlayer;

    private POSITION3 mLastPlayerPOS = POSITION3.NONE;

    [SerializeField] private   Floor[] mFloors = new Floor[1];
    [SerializeField] private   Floor   mCurrentFloor;
                     private Vector2[] mMovePoints;

    public Vector2 GetMovePoint(DIRECTION9 direction)
    {
        return mMovePoints[(int)direction];
    }

    public void PlayerRegister(uint floor, Player player)
    {
        if (mFloors[floor] != null)
        {
            mCurrentFloor = mFloors[floor];

            mEyePlayer = player;
            
            Renew();
        }
    }

    private void Renew()
    {
        mCurrentFloor.IInit();

        if (mEyePlayer)
        {
            RenewPlayerPOS();
        }
        Vector2[] topMovePoint = mCurrentFloor.GetMovePoints(POSITION3.TOP);
        Vector2[] midMovePoint = mCurrentFloor.GetMovePoints(POSITION3.MID);
        Vector2[] botMovePoint = mCurrentFloor.GetMovePoints(POSITION3.BOT);

        mMovePoints = new Vector2[(int)DIRECTION9.END]
        {
            topMovePoint[0], topMovePoint[1], topMovePoint[2],
            midMovePoint[0], midMovePoint[1], midMovePoint[2],
            botMovePoint[0], botMovePoint[1], botMovePoint[2]
        };
    }

    private void RenewPlayerPOS()
    {
        if(mLastPlayerPOS != mEyePlayer.GetPOSITION9())
        {
            if (mLastPlayerPOS != POSITION3.NONE)
            {
                mCurrentFloor.ExitPlayer(mLastPlayerPOS);
            }
            mCurrentFloor.EnterPlayer(mLastPlayerPOS = mEyePlayer.GetPOSITION9());
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

    private void BuildCastle()
    {
        for (int i = 0; i < mFloors.Length; ++i)
        {
            mFloors[i].BuildRoom();
        }
    }

    private void Start()
    {
        StartCoroutine(CR_update());

        BuildCastle();
    }
}
