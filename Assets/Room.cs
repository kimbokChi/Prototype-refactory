using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public  Transform[]  MovePoints = new Transform[3];
    private   Vector2[] mMovePoints = new   Vector2[3];

    private Floor mMasterFloor;


    public void IInit(Floor masterFloor)
    {
        mMasterFloor = masterFloor;

        mMovePoints[0] = MovePoints[0].position;
        mMovePoints[1] = MovePoints[1].position;
        mMovePoints[2] = MovePoints[2].position;
        Debug.Log("방 초기화!");
    }

    public void IUpdate()
    {
        // todo. . .
        Debug.Log("방 업데이트!");
    }
}
