using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObserver : Singleton<GameObserver>
{
    public float StartTime
    {
        get;
        private set;
    }
    public float ResultTime
    {
        get;
        private set;
    }
    public int KillCount
    {
        get;
        private set;
    }

    public void KillEnemy()
    {
        KillCount++;
    }
    public void GameClear()
    {
        ResultTime = Time.time - StartTime;
    }

    private void Start()
    {
        KillCount = 0;

        StartTime = Time.time;
    }
}
