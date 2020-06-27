using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    private Floor mMasterFloor;

    public void Init(Floor masterFloor)
    {
        mMasterFloor = masterFloor;
    }

    private IEnumerator CR_update()
    {
        while(mMasterFloor.IsOnPlayer)
        {


            yield return new WaitUntil(() => mMasterFloor.CanUpdate);
            yield return null;
        }
        yield break;
    }
}
