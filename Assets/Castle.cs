using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Castle : Singleton<Castle>
{
    private bool mIsActivation = true;

    private Player mPlayer;

    private LPOSITION3 mLastPlayerPOS = LPOSITION3.NONE;

    [SerializeField] private   Floor[] mFloors;
    [SerializeField] private   Floor   mPlayerFloor;
                     private Vector2[] mMovePoints;

    #region READ
    /// <summary>
    /// 현재 층안에 존재하는 이동 지점의 위치를 반한합니다.
    /// </summary>
    #endregion 
    public Vector2 GetMovePoint(DIRECTION9 direction)
    {
        return mMovePoints[(int)direction];
    }

    #region READ
    /// <summary>
    /// 다음 층으로 이동할 수 있는지의 여부를 반환합니다.
    /// </summary>
    #endregion 
    public bool CanNextPoint()
    {
        return !(IsIndexOutFloor(mPlayerFloor.FloorIndex)) && mPlayerFloor.IsClear;
    }

    #region READ
    /// <summary>
    /// 다음 층으로 이동할 수 있는지의 여부를 반환합니다.
    /// </summary>
    /// <param name="point">다음층의 이동 지점을 저장할 변수를 사용합니다</param>
    /// <returns></returns>
    #endregion 
    public bool CanNextPoint(out Vector2 point)
    {
        if (IsIndexOutFloor(mPlayerFloor.FloorIndex))
        {
            point = Vector2.zero; return false;
        }
        else
        {
            Floor moveFloor = mFloors[mPlayerFloor.FloorIndex];

            int playerPOS = (int)mPlayer.GetTPOSITION3();

            point = moveFloor.GetMovePoints(LPOSITION3.BOT)[playerPOS];

            return true;
        }
    }

    #region READ
    /// <summary>
    /// 층간이동으로 플레이어가 이동한 층을 활성화시킵니다.
    /// </summary>
    #endregion 
    public void AliveNextPoint()
    {
        // 위 지역으로 이동할 수 있을때
        if (!IsIndexOutFloor(mPlayerFloor.FloorIndex)) 
        {
            mPlayerFloor = mFloors[mPlayerFloor.FloorIndex];

            Renew();
        }
    }

    #region _MEMBER
    /// <summary>
    /// 멤버함수 : 지정한 인덱스의 층이 존재하는지의 여부를 반환합니다.
    /// </summary>
    #endregion 
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

    private void PlayerCheck(int floor)
    {
        if (mFloors[floor - 1] != null)
        {
            mPlayerFloor = mFloors[floor - 1];

            mPlayer = FindObjectOfType(typeof(Player)) as Player;
        }
    }

    #region _MEMBER
    /// <summary>
    /// 플레이어가 존재하는 층인 mPlayerFloor를 가동시키고,
    /// <para>멤버변수들의 정보를 mPlayerFloor에 대한 정보로 갱신합니다.</para>
    /// </summary>
    #endregion 
    private void Renew()
    {
        mPlayerFloor.IInit();

        if (mPlayer)
        {
            RenewPlayerPOS();
        }
        Vector2[] topMovePoint = mPlayerFloor.GetMovePoints(LPOSITION3.TOP);
        Vector2[] midMovePoint = mPlayerFloor.GetMovePoints(LPOSITION3.MID);
        Vector2[] botMovePoint = mPlayerFloor.GetMovePoints(LPOSITION3.BOT);

        mMovePoints = new Vector2[(int)DIRECTION9.END]
        {
            topMovePoint[0], topMovePoint[1], topMovePoint[2],
            midMovePoint[0], midMovePoint[1], midMovePoint[2],
            botMovePoint[0], botMovePoint[1], botMovePoint[2]
        };
    }
    #region _MEMBER
    /// <summary>
    /// 현재 플레이어가 있는 층을 가리키는 mPlayerFloor를 가동시키고,
    /// <para>멤버변수들의 정보를 mPlayerFloor에 대한 정보로 갱신합니다.</para>
    /// </summary>
    #endregion
    private void RenewPlayerPOS()
    {
        if (mLastPlayerPOS != mPlayer.GetLPOSITION3())
        {
            if (mLastPlayerPOS != LPOSITION3.NONE)
            {
                mPlayerFloor.ExitPlayer(mLastPlayerPOS);
            }
            mPlayerFloor.EnterPlayer(mLastPlayerPOS = mPlayer.GetLPOSITION3());
        }
    }

    private IEnumerator CR_update()
    {
        while (mIsActivation)
        {
            if (mPlayer)
            {
                RenewPlayerPOS();
            }
            if (mPlayerFloor)
            {
                mPlayerFloor.IUpdate();
            }
            yield return null;
        }
        yield break;
    }

    private void Start()
    {
        BuildCastle();

        PlayerCheck(1);

        Renew();

        StartCoroutine(CR_update());
    }
}
