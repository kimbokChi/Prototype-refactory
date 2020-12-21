using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyManager : Singleton<MoneyManager>
{
    [SerializeField]
    private TMPro.TextMeshProUGUI MoneyText;

    public int Money
    {
        get;
        private set;
    }
    public void TextUpdate()
    {
        MoneyText.text = Money.ToString();
    }

    public void SetMoney(int money)
    {
        Money = money;

        TextUpdate();
    }
    public void AddMoney(int money)
    {
        Money += money;

        TextUpdate();
    }
    public bool SubtractMoney(int money)
    {
        if (Money >= money)
        {
            Money -= money;
            TextUpdate();

            return true;
        }
        return false;
    }
}
