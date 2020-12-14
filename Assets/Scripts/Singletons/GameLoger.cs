using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoger : Singleton<GameLoger>
{
    public float StartTime
    {
        get;
        private set;
    }
    public float ElapsedTime
    {
        get => Time.time - StartTime;
    }
    public int KillCount
    {
        get;
        private set;
    }

    public void EnemyDead()
    {
        KillCount++;
    }
    private void Start()
    {
        KillCount = 0;

        StartTime = Time.time;
    }
}
