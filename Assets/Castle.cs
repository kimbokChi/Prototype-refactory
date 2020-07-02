using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Castle : Singleton<Castle>
{
    private bool mIsActivation = true;

    [SerializeField] private   Floor   mCurrentFloor;
                     private Vector2[] mMovePoints;

    public Vector2 GetMovePoint(DIRECTION9 direction)
    {
        return mMovePoints[(int)direction];
    }

    private IEnumerator CR_update()
    {
        mCurrentFloor.IInit();

        Vector2[] topMovePoint = mCurrentFloor.GetMovePoints(POSITION3.TOP);
        Vector2[] midMovePoint = mCurrentFloor.GetMovePoints(POSITION3.MID);
        Vector2[] botMovePoint = mCurrentFloor.GetMovePoints(POSITION3.BOT);

        mMovePoints = new Vector2[(int)DIRECTION9.END]
        {
            topMovePoint[0], topMovePoint[1], topMovePoint[2],
            midMovePoint[0], midMovePoint[1], midMovePoint[2],
            botMovePoint[0], botMovePoint[1], botMovePoint[2]
        };

        while (mIsActivation)
        {
            if (mCurrentFloor)
            {
                mCurrentFloor.IUpdate();
            }
            yield return null;
        }
        yield break;
    }

    private void Start()
    {
        StartCoroutine(CR_update());
    }
}
