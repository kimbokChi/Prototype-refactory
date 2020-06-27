using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    private Floor mMasterFloor;

    public void Init(Floor masterFloor)
    {
        mMasterFloor = masterFloor;

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
