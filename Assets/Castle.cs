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

        mMovePoints = new Vector2[(int)DIRECTION9.END]
        {
            mCurrentFloor.GetMovePoints(POSITION3.TOP)[0],
            mCurrentFloor.GetMovePoints(POSITION3.TOP)[1],
            mCurrentFloor.GetMovePoints(POSITION3.TOP)[2],

            mCurrentFloor.GetMovePoints(POSITION3.MID)[0],
            mCurrentFloor.GetMovePoints(POSITION3.MID)[1],
            mCurrentFloor.GetMovePoints(POSITION3.MID)[2],

            mCurrentFloor.GetMovePoints(POSITION3.BOT)[0],
            mCurrentFloor.GetMovePoints(POSITION3.BOT)[1],
            mCurrentFloor.GetMovePoints(POSITION3.BOT)[2]
        };

        for(int i = 0; i < (int)DIRECTION9.END; ++i)
        {
            GameObject game = new GameObject("AAA");

            game.transform.position = mMovePoints[i];
        }

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
