using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Castle : Singleton<Castle>
{
    private bool mIsActivation = true;

    private Player mEyePlayer;

    private POSITION3 mLastPlayerPOS = POSITION3.NONE;

    [SerializeField] private   Floor[] mFloors = new Floor[10];
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

            mLastPlayerPOS = POSITION3.BOT;

            Renew();
        }
    }

    private void Renew()
    {
        mCurrentFloor.IInit();

        if (mEyePlayer)
        {
            if (mEyePlayer.GetPOSITION9() != mLastPlayerPOS)
            {
                mCurrentFloor.ExitPlayer (mLastPlayerPOS);

                mCurrentFloor.EnterPlayer(mLastPlayerPOS = mEyePlayer.GetPOSITION9());
            }
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

    private IEnumerator CR_update()
    {
        while (mIsActivation)
        {
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
        StartCoroutine(CR_update());
    }
}
