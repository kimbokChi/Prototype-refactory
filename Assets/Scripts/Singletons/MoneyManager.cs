using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyManager : Singleton<MoneyManager>
{
    public int Money
    {
        get;
        private set;
    }

    public void Addition(int increasement)
    {
        Money += increasement;
    }
    public bool Subtract(int decreasement)
    {
        if (Money >= decreasement)
        {
            Money -= decreasement;

            return true;
        }
        return false;
    }
}
