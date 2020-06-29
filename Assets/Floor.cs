using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    [SerializeField]
    private Room[] mMemberRooms = new Room[3];

    public void IInit()
    {
        mMemberRooms[0].IInit(this);
        mMemberRooms[1].IInit(this);
        mMemberRooms[2].IInit(this);
        Debug.Log("층 초기화!");
    }

    public void IUpdate()
    {
        mMemberRooms[0].IUpdate();
        mMemberRooms[1].IUpdate();
        mMemberRooms[2].IUpdate();
        Debug.Log("층 업데이트!");
    }
}
