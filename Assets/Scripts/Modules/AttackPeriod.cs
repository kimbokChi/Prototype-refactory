using System;
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


    public AttackPeriod(AbilityTable abilityTable, float attackDelayTime = 0f)
    {
        mWaitPeriod = Period.Begin;

        mAttackDelayTime = attackDelayTime;

        (mTimer = new Timer()).Start(abilityTable.AfterAttackDelay);

        mAbilityTable = abilityTable;
    }

    public void SetAction
        (Action enterBeginAction, Action enterAttackAction, Action enterAfterAction)
    {
        mEnterBeginAction = enterBeginAction;
        mEnterAfterAction = enterAfterAction;

        mEnterAttackAction = enterAttackAction;
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

    public void Update()
    {
        mTimer.Update();

        if (mTimer.IsOver())
        {
            switch (mWaitPeriod)
            {
                case Period.Begin:

                    mEnterBeginAction?.Invoke();

                    mWaitPeriod = Period.Attack;
                    mTimer.Start(mAbilityTable.BeginAttackDelay);
                    break;

                case Period.Attack:

                    mEnterAttackAction?.Invoke();

                    mWaitPeriod = Period.After;
                    mTimer.Start(mAttackDelayTime);
                    break;

                case Period.After:

                    mEnterAfterAction?.Invoke();

                    mWaitPeriod = Period.Begin;
                    mTimer.Start(mAbilityTable.AfterAttackDelay);
                    break;
            }
        }
    }
}
