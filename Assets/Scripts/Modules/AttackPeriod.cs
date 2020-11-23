using System;
using System.Collections;
using UnityEngine;
using Mono = Inventory;

public enum Period { Begin, Attack, After };

public class AttackPeriod
{
    private Action mEnterBeginAction;
    private Action mEnterAfterAction;
    private Action mEnterAttackAction;

    private AbilityTable mAbilityTable;

    private IEnumerator mEUpdate;
    private bool mIsAttackOver;


    public AttackPeriod(AbilityTable abilityTable)
    {
        mAbilityTable = abilityTable;
    }
    public void AttackActionOver()
    {
        mIsAttackOver = true;
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
            Inventory.Instance.StartCoroutine(mEUpdate = EUpdate());
        }
    }
    public void StopPeriod()
    {
        if (mEUpdate != null) {
            Inventory.Instance.StopCoroutine(mEUpdate);
        }
    }

    private IEnumerator EUpdate()
    {
        mIsAttackOver = false;

        float DeltaTime() {
            return Time.deltaTime * Time.timeScale;
        }
        mEnterBeginAction?.Invoke();
        for (float w = 0f; w < mAbilityTable.BeginAttackDelay; w += DeltaTime())
            yield return null;

        mEnterAttackAction?.Invoke();
        yield return new WaitUntil(() => mIsAttackOver);

        mEnterAfterAction?.Invoke();
        for (float w = 0f; w < mAbilityTable.AfterAttackDelay; w += DeltaTime())
            yield return null;

        mEUpdate = null;
        mIsAttackOver = false;
    }
}
