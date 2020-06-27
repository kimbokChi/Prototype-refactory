using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    private Room[] mMemberRooms = new Room[3];
    
    public  bool  CanUpdate
    {
        get { return mCanUpdate; }
    }
    private bool mCanUpdate = true;

    public  bool  IsOnPlayer
    {
        get { return mIsOnPlayer; }
    }
    private bool mIsOnPlayer = false;

    public void Init()
    {
        // mMemberRooms배열을 여기서 초기화한다
    }
}
