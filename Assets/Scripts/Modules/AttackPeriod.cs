using System;
public class AttackPeriod
{
    private enum Period { Begin, After };


    private Period mWaitPeriod;

    private Timer mTimer;

    private Action mBeginOverAction;
    private Action mAfterOverAction;

    private AbilityTable mAbilityTable;


    public AttackPeriod(AbilityTable abilityTable,
        Action beginOverAction = null,
        Action afterOverAction = null)
    {
        mWaitPeriod = Period.Begin;

        (mTimer = new Timer()).Start(abilityTable.AfterAttackDelay);

        mAbilityTable = abilityTable;

        mBeginOverAction = beginOverAction;
        mAfterOverAction = afterOverAction;
    }
    public void Update()
    {
        mTimer.Update();

        if (mTimer.IsOver())
        {
            if (mWaitPeriod.Equals(Period.Begin))
            {
                mBeginOverAction?.Invoke();

                mWaitPeriod = Period.After;
                mTimer.Start(mAbilityTable.AfterAttackDelay);
            }
            else
            {
                mAfterOverAction?.Invoke();

                mWaitPeriod = Period.Begin;
                mTimer.Start(mAbilityTable.BeginAttackDelay);
            }
        }
    }
}
