using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoADPurchaser : MonoBehaviour
{
    public void PurchaseSuccess()
    {
        IAP.Instance.Reward();
    }
    public void PurchaseFaild()
    {
        IAP.Instance.Faild();
    }
}
