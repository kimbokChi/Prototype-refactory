using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinDropper : MonoBehaviour
{
    [SerializeField] private int _CoinValue;
    [SerializeField] private float _GroundOffsetY;

    public void Drop(int count = 1)
    {
        Vector3 offset = Vector3.up * _GroundOffsetY;

        for (int i = 0; i < count; i++)
        {
            Coin coin = CoinPool.Instance.Get();

            coin.Value = _CoinValue;

            coin.transform.position = transform.position + offset;
            coin.Enable();
        }
    }
}
