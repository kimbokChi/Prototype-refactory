using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField][Tooltip("좌측 이동 지점입니다")] private Transform LMovePoint; 
    [SerializeField][Tooltip("중심 이동 지점입니다")] private Transform CMovePoint;
    [SerializeField][Tooltip("우측 이동 지점입니다")] private Transform RMovePoint; 

    private Floor mMasterFloor;

                     public  ROOM_NUMBER  RoomNumber
    {
        get { return mRoomNumber; }
    }
    [SerializeField] private ROOM_NUMBER mRoomNumber;

    // 해당 객실에 귀속된 오브젝트들을 저장합니다
    private List<IObject> mObjects = new List<IObject>();

    public void IInit(Floor masterFloor)
    {
        mMasterFloor = masterFloor;

        IObject Object;

        for (int i = 0; i < transform.childCount; ++i)
        {
            if (transform.GetChild(i).TryGetComponent(out Object))
            {
                mObjects.Add(Object);

                Object.IInit();
            }
        }
        gameObject.SetActive(false);
    }

    public void IUpdate()
    {
        for (int i = 0; i < mObjects.Count; ++i)
        {
            mObjects[i].IUpdate();
        }
    }

    public void EnterPlayer()
    {
        // 플레이어가 해당 방에 존재한다!
    }
    public void ExitPlayer()
    {
        // 플레이어가 나갔다..
    }

    public Vector2[] GetMovePoints()
    {
        return new Vector2[3] { LMovePoint.position, CMovePoint.position, RMovePoint.position };
    }
}
