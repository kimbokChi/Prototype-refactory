using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropper : MonoBehaviour
{
    [Header("Coin Property")]
    [SerializeField] private float _GroundOffsetY;

    public void CoinDrop(int count = 1)
    {
        Vector3 offset = Vector3.up * _GroundOffsetY;

        for (int i = 0; i < count; i++)
        {
            Coin coin = CoinPool.Instance.Get();

            coin.Value = 1;

            coin.transform.position = transform.position + offset;
            coin.Enable();
        }
    }
}
