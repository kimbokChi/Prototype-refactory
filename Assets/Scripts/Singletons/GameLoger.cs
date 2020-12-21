using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoger : Singleton<GameLoger>
{
    [ContextMenu("Add Money")]
    private void AAA()
    {
        MoneyManager.Instance.AddMoney(30);
    }

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
    public int RecordedMoney
    {
        get;
        private set;
    }


    public void EnemyDead()
    {
        KillCount++;
    }
    public void RecordMoney(int money)
    {
        RecordedMoney = money;
    }

    private void Awake()
    {
        if (GameObject.FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += (a, b) =>
            {
                KillCount = 0;

                StartTime = Time.time;
            };
        }
    }
}
