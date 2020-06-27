using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    private const float MOVEPOINT_OFFSET = 2.5f;


    private Floor mMasterFloor;

    private Vector2[] mMovePoints = new Vector2[3];

    public void Init(Floor masterFloor)
    {
        mMasterFloor = masterFloor;

        // 0번째 인덱스 : 왼쪽 지점 || 1번째 인덱스 : 가운데 지점 || 2번째 인덱스 : 오른쪽 지점
        for (int i = 1; i > -2; --i)
        {
            mMovePoints[i] = (Vector2)transform.position + (Vector2.left * MOVEPOINT_OFFSET * i);
        }
        StartCoroutine(CR_update());
    }

    private IEnumerator CR_update()
    {
        while (mMasterFloor.IsOnPlayer)
        {
            Debug.Log("흐-른-다-!");

            yield return new WaitUntil(() => mMasterFloor.CanUpdate);
            yield return null;
        }

        Debug.Log("시스템 종료오오옯ㅇ..");
        yield break;
    }
}
