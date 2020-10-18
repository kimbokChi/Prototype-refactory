using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Castle : Singleton<Castle>
{
    [SerializeField][Range(0.1f, 2f)]
    private float mCamaraMoveAccel;

    private bool mIsCastClearEvent;
    private bool mIsActivation = true;
    private bool mIsPause   = false;

    private Player mPlayer;

    private LPOSITION3 mLastPlayerPOS = LPOSITION3.NONE;

    [SerializeField] private   Floor[] mFloors;
    [SerializeField] private   Floor   mPlayerFloor;
                     private Vector2[] mMovePoints;

    private IEnumerator mECamaraMove;

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
            mPlayerFloor = mFloors[mPlayerFloor.FloorIndex];

            int playerPOS = (int)mPlayer.GetTPOSITION3();

            point = mPlayerFloor.GetMovePoints(LPOSITION3.BOT)[playerPOS];

            RenewPlayerFloor();

            if (mECamaraMove != null)
            {
                StopCoroutine(mECamaraMove);
            }
            StartCoroutine(mECamaraMove = ECamaraMove(mPlayerFloor.transform.position, Camera.main));

            return true;
        }
    }

    public void PauseEnable() => mIsPause = true;

    public void PauseDisable() => mIsPause = false;

    public Room[] GetFloorRooms()
    {
        return mPlayerFloor.GetRooms();
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

    #region _MEMBER
    /// <summary>
    /// 플레이어가 존재하는 층인 mPlayerFloor를 가동시키고,
    /// <para>멤버변수들의 정보를 mPlayerFloor에 대한 정보로 갱신합니다.</para>
    /// </summary>
    #endregion 
    private void RenewPlayerFloor()
    {
        mPlayerFloor.IInit();

        mPlayerFloor.EnterPlayer(mPlayer);

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
                mPlayerFloor.ExitPlayer(MESSAGE.THIS_ROOM, mLastPlayerPOS);
            }
            mPlayerFloor.EnterPlayer(mPlayer, mLastPlayerPOS = mPlayer.GetLPOSITION3());
        }
    }

    private IEnumerator ECamaraMove(Vector2 movePoint, Camera camera)
    {
        float lerpAmount = 0f;

        while (lerpAmount < 1f)
        {
            lerpAmount = Mathf.Min(1f, lerpAmount + Time.deltaTime * Time.timeScale * mCamaraMoveAccel);

            camera.transform.position = Vector2.Lerp(camera.transform.position, movePoint, lerpAmount);

            camera.transform.Translate(0, 0, -10f);

            yield return null;
        }
        mECamaraMove = null;

        yield break;
    }

    private IEnumerator EUpdate()
    {
        while (mIsActivation)
        {
            if (!mIsPause)
            {
                if (mPlayer)
                {
                    RenewPlayerPOS();
                }
                if (mPlayerFloor)
                {
                    mPlayerFloor.IUpdate();
                }
                if (mPlayerFloor.IsClear && !mIsCastClearEvent)
                {
                    mIsCastClearEvent = true;

                    StageEventLibrary.Instance.NotifyStageClear();
                }
                if (mPlayer.IsDeath)
                {
                    mPlayerFloor.ExitPlayer(MESSAGE.BELONG_FLOOR, mLastPlayerPOS);
                }
            }
            yield return null;
        }
        yield break;
    }

    private void Start()
    {
        mIsCastClearEvent = false;

        BuildCastle();

        mPlayer = FindObjectOfType(typeof(Player)) as Player;

        RenewPlayerFloor();

        StartCoroutine(EUpdate());
    }
}
