using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    [SerializeField]
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
    private bool mIsOnPlayer = true;

    private void Start() => Init();

    public void Init()
    {
        // mMemberRooms배열을 여기서 초기화한다

        for(int i = 0; i < 3; ++i)
        {
            mMemberRooms[i].Init(this);
        }

        StartCoroutine(CR_update());
    }

    private IEnumerator CR_update()
    {
        while(gameObject.activeSelf)
        {
            if(Input.GetMouseButtonDown(0))
            {
                mIsOnPlayer = !mIsOnPlayer;
            }
            if (Input.GetMouseButtonDown(1))
            {
                mCanUpdate = !mCanUpdate;
            }
            yield return null;
        }
        yield break;
    }
}
