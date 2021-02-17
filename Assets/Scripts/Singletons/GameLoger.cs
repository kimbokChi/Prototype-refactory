﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoger : Singleton<GameLoger>
{
    [ContextMenu("Add Money")]
    private void AAA()
    {
        MoneyManager.Instance.AddMoney(300);

        Vector2 point =
            (Vector2)GameObject.FindGameObjectWithTag("Player").transform.position + new Vector2(0, -0.5f);

        EffectLibrary.Instance.UsingEffect(EffectKind.Coin, point);
        EffectLibrary.Instance.UsingEffect(EffectKind.Coin, point);
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
    public int UnlockDungeonIndex
    {
        get;
        private set;
    }

    [HideInInspector] public float ControllerOffset;
    [HideInInspector] public float ControllerDefScale;
    [HideInInspector] public float ControllerMaxScale;
    [HideInInspector] public float ControllerAlpha;
    [HideInInspector] public Vector2 ControllerPos;

    private const float ControllerOFFSET = 90f;
    private const float ControllerDEFSCALE = 0.65f;
    private const float ControllerMAXSCALE = 0.7f;
    private const float ControllerALPHA = 0.5f;
    private readonly Vector2 ControllerPOS = new Vector2(0, -380f);

    public void EnemyDead()
    {
        KillCount++;
    }
    public void RecordMoney(int money)
    {
        RecordedMoney = money;
    }
    public void RecordStageUnlock()
    {
        UnlockDungeonIndex++;
    }
    public void SetStageUnlock(int index)
    {
        UnlockDungeonIndex = index;
    }

    private void Awake()
    {
        if (GameObject.FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            UnlockDungeonIndex = 0;

            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += (a, b) =>
            {
                KillCount = 0;

                StartTime = Time.time;
            };
            ControllerOffset = ControllerOFFSET;
            ControllerDefScale = ControllerDEFSCALE;
            ControllerMaxScale = ControllerMAXSCALE;
            ControllerAlpha = ControllerALPHA;
            ControllerPos = ControllerPOS;
        }
    }
}
