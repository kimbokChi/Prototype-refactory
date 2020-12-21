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

    public void Addition(int increasement)
    {
        Money += increasement;

        TextUpdate();
    }
    public bool Subtract(int decreasement)
    {
        if (Money >= decreasement)
        {
            Money -= decreasement;
            TextUpdate();

            return true;
        }
        return false;
    }
}
