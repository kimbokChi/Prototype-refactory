using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private float mWaitTime    = 0.0f;
    private float mWaitTimeSum = 0.0f;

    public void SetTimer(float waitTime) => mWaitTime = waitTime;

    public bool IsOver()
    {
        mWaitTimeSum += Time.deltaTime * Time.timeScale;

        if (mWaitTimeSum >= mWaitTime)
        {
            mWaitTimeSum = 0.0f; return true;
        }
        return false;
    }
}
