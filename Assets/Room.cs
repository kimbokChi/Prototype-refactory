using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField][Tooltip("좌측 이동 지점입니다")] private Transform LMovePoint; 
    [SerializeField][Tooltip("중심 이동 지점입니다")] private Transform CMovePoint;
    [SerializeField][Tooltip("우측 이동 지점입니다")] private Transform RMovePoint; 

    private Floor mMasterFloor;

    public void IInit(Floor masterFloor)
    {
        mMasterFloor = masterFloor;

        gameObject.SetActive(true);
    }

    public void IUpdate()
    {
    
    }

    public void EnterPlayer()
    {
        Debug.Log("플레ㅣ어께서 존재하신다!");
        // 플레이어가 해당 방에 존재한다!
    }
    public void ExitPlayer()
    {
        Debug.Log("이젠 아니야...");
        // 플레이어가 나갔다..
    }

    public Vector2[] GetMovePoints()
    {
        return new Vector2[3] { LMovePoint.position, CMovePoint.position, RMovePoint.position };
    }
}
