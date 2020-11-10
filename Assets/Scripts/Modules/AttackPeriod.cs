using System;
using System.Collections;
using UnityEngine;
using Mono = Inventory;

public enum Period { Begin, Attack, After };

public class AttackPeriod
{
    private Period mWaitPeriod;

    private Timer mTimer;

    private Action mEnterBeginAction;
    private Action mEnterAfterAction;
    private Action mEnterAttackAction;

    private AbilityTable mAbilityTable;

    private float mAttackDelayTime;

    private IEnumerator mEUpdate;


    public AttackPeriod(AbilityTable abilityTable, float attackDelayTime = 0f)
    {
        mWaitPeriod = Period.Begin;

        mAttackDelayTime = attackDelayTime;

        (mTimer = new Timer()).Start(abilityTable.AfterAttackDelay);

        mAbilityTable = abilityTable;
    }

    public void SetAttackTime(float time) 
    {
        mAttackDelayTime = time;
    }

    public bool IsProgressing()
    {
        return mEUpdate != null;
    }
    public void SetAction(Period period, Action action)
    {
        switch (period)
        {
            case Period.Begin: mEnterBeginAction = action;
                break;

            case Period.After: mEnterAfterAction = action;
                break;

            case Period.Attack: mEnterAttackAction = action;
                break;
        }
    }

    public void StartPeriod()
    {
        if (mEUpdate == null) {
            Mono.Instance.StartCoroutine(mEUpdate = EUpdate());
        }
    }
    public void StopPeriod()
    {
        if (mEUpdate != null) {
            Mono.Instance.StopCoroutine(mEUpdate);
        }
    }

    private IEnumerator EUpdate()
    {
        float DeltaTime() {
            return Time.deltaTime * Time.timeScale;
        }

        Action[] enterActions = new Action[3]
        {
            mEnterBeginAction, mEnterAttackAction, 
            mEnterAfterAction
        };
        float[] delays = new float[3]
        {
            mAbilityTable.BeginAttackDelay, mAttackDelayTime, 
            mAbilityTable.AfterAttackDelay
        };

        for (int i = 0; i < 3; i++)
        {
            enterActions[i]?.Invoke();

            for (float w = 0f; w < delays[i]; w += DeltaTime()) {
                yield return null;
            }
        }
        mEUpdate = null;
    }
}
