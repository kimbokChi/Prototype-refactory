using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    private float mWaitTime    = 0.0f;
    private float mWaitTimeSum = 0.0f;

    public void Start(float waitTime)
    {
        mWaitTime = waitTime;

        mWaitTimeSum = 0.0f;
    }

    public void Update()
    {
        mWaitTimeSum += Time.deltaTime * Time.timeScale;
    }

    public bool IsOver() => mWaitTimeSum >= mWaitTime;

    public float TimeOverFlow() => mWaitTime - mWaitTimeSum;
}
