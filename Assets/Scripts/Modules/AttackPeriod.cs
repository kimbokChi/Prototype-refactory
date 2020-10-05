using System;
public class AttackPeriod
{
    private enum Period { Begin, Attack, After };


    private Period mWaitPeriod;

    private Timer mTimer;

    private Action mEnterBeginAction;
    private Action mEnterAfterAction;
    private Action mEnterAttackAction;

    private AbilityTable mAbilityTable;


    public AttackPeriod(AbilityTable abilityTable,
        Action enterBeginAction = null, Action enterAttackAction = null,
        Action enterAfterAction = null)
    {
        mWaitPeriod = Period.Begin;

        (mTimer = new Timer()).Start(abilityTable.AfterAttackDelay);

        mAbilityTable = abilityTable;

        mEnterBeginAction = enterBeginAction;
        mEnterAfterAction = enterAfterAction;

        mEnterAttackAction = enterAttackAction;
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
                    mTimer.Start(mAbilityTable.AttackDelay);
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
