using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField][Tooltip("좌측 이동 지점입니다")] private Transform LMovePoint; 
    [SerializeField][Tooltip("중심 이동 지점입니다")] private Transform CMovePoint;
    [SerializeField][Tooltip("우측 이동 지점입니다")] private Transform RMovePoint; 

    private Floor mBelongFloor;

    public  bool  IsClear => mIsClear;
    private bool mIsClear;

    public int BelongFloorIndex
    {
        get
        {
            if (mBelongFloor != null)
            {
                return mBelongFloor.FloorIndex;
            }
            return 0;
        }
    }

                     public  ROOM_NUMBER  RoomNumber => mRoomNumber;
    [SerializeField] private ROOM_NUMBER mRoomNumber;

    private List<IObject> mObjects;

    public void IInit(Floor parentFloor)
    {
        mBelongFloor = parentFloor;

        mObjects = new List<IObject>();

        for (int i = 0; i < transform.childCount; ++i)
        {
            if (transform.GetChild(i).TryGetComponent(out IObject Object))
            {
                mObjects.Add(Object);

                Object.IInit();
            }
        }
        gameObject.SetActive(false);

        mIsClear = false;
    }

    public void IUpdate()
    {
        if (!mIsClear)
        {
            int activeCount = 0;

            for (int i = 0; i < mObjects.Count; ++i)
            {
                if (mObjects[i].IsActive())
                {
                    mObjects[i].IUpdate();

                    activeCount++;
                }
            }
            if (activeCount == 0)
            {
                mIsClear = true;
            }
        }
    }

    public void EnterPlayer(MESSAGE message, Player enterPlayer)
    {
        for (int i = 0; i < mObjects.Count; ++i)
        {
            mObjects[i].PlayerEnter(message, enterPlayer);
        }
    }
    public void ExitPlayer(MESSAGE message)
    {
        for (int i = 0; i < mObjects.Count; ++i)
        {
            mObjects[i].PlayerExit(message);
        }
    }
    
    #region READ
    /// <summary>
    /// 해당 방에게 종속될 오브젝트를 추가합니다.
    /// <para>주로 방 내에 있는 개체가 새로운 개체를 만들어 냈을 때에 사용됩니다.</para>
    /// </summary>
    /// <param name="object">해당 방에 추가될 오브젝트의 IObject인터페이스를 지정합니다</param>
    #endregion
    public void AddIObject(IObject @object)
    {
        mObjects.Add(@object);

        @object.IInit();
    }

    public Vector2[] GetMovePoints()
    {
        return new Vector2[3] { LMovePoint.position, CMovePoint.position, RMovePoint.position };
    }
}
