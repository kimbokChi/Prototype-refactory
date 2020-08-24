using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    private Room[] mMemberRooms = new Room[3];

    [SerializeField]
    [Tooltip("높이있는 방일수록 가장 작은 인덱스에 저장해 주세요!")]
    private Transform[] mRoomPoints = new Transform[3];

                     public  int  FloorIndex => mFloorIndex;
    [SerializeField] private int mFloorIndex;

    public  bool  IsClear => mIsClear;
    private bool mIsClear;

    public void IInit()
    {
        mMemberRooms[0].gameObject.SetActive(true);
        mMemberRooms[1].gameObject.SetActive(true);
        mMemberRooms[2].gameObject.SetActive(true);

        mIsClear = false;
    }

    public void IUpdate()
    {
        if (!mIsClear)
        {
            int clearRoomCount = 0;

            for (int i = 0; i < mMemberRooms.Length; ++i)
            {
                mMemberRooms[i].IUpdate();

                if (mMemberRooms[i].IsClear)
                {
                    clearRoomCount++;
                }
            }
            if (clearRoomCount == mMemberRooms.Length)
            {
                mIsClear = true;
            }
        }
    }

    #region READ
    /// <summary>
    /// 해당 층을 구성할 방 객체들을 생성합니다.
    /// </summary>
    #endregion
    public void BuildRoom()
    {
        for (int i = 0; i < mMemberRooms.Length; ++i)
        {
            mMemberRooms[i] = Instantiate(RoomLibrary.Instnace.Random(), mRoomPoints[i].position, Quaternion.identity);

            mMemberRooms[i].IInit(this);
        }
    }

    public void EnterPlayer(Player player)
    {
        mMemberRooms[0].EnterPlayer(MESSAGE.BELONG_FLOOR, player);
        mMemberRooms[1].EnterPlayer(MESSAGE.BELONG_FLOOR, player);
        mMemberRooms[2].EnterPlayer(MESSAGE.BELONG_FLOOR, player);
    }
    public void EnterPlayer(Player player, LPOSITION3 position)
    {
        mMemberRooms[(int)position].EnterPlayer(MESSAGE.THIS_ROOM, player);
    }
    public void ExitPlayer(MESSAGE message, LPOSITION3 position)
    {
        switch (message)
        {
            case MESSAGE.THIS_ROOM:
                mMemberRooms[(int)position].ExitPlayer(MESSAGE.THIS_ROOM);
                break;

            case MESSAGE.BELONG_FLOOR:
                mMemberRooms[0].ExitPlayer(MESSAGE.BELONG_FLOOR);
                mMemberRooms[1].ExitPlayer(MESSAGE.BELONG_FLOOR);
                mMemberRooms[2].ExitPlayer(MESSAGE.BELONG_FLOOR);
                break;
        }
    }

    #region READ
    /// <summary>
    /// 지정한 방에 존재하는 이동지점들을 반환합니다.
    /// </summary>
    #endregion
    public Vector2[] GetMovePoints(LPOSITION3 position)
    {
        return mMemberRooms[(int)position].GetMovePoints();
    }

    public Transform[] GetRoomTransforms()
    {
        return new Transform[3] { mMemberRooms[0].transform, mMemberRooms[1].transform, mMemberRooms[2].transform };
    }
}
