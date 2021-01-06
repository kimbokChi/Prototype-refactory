using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackButtonHider : MonoBehaviour
{
    private bool _ButtonAlreadyOpen;
    private bool _IsAlreadyInit;

    public void HideOrShow()
    {
        if (!_IsAlreadyInit)
        {
            _ButtonAlreadyOpen = AttackButtonControlar.Instance.IsButtonHide;

            if (_ButtonAlreadyOpen)
            {
                AttackButtonControlar.Instance.HideButton();
            }
            _IsAlreadyInit = true;
        }
        else
        {
            if (_ButtonAlreadyOpen)
            {
                AttackButtonControlar.Instance.ShowButton();
            }
            _IsAlreadyInit = false;
        }
    }
}
