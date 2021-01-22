using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int Value;

    [SerializeField] private Animator _Animator;
    private int _AnimationHash;

    private void OnEnable()
    {
        _AnimationHash = _Animator.GetParameter(0).nameHash;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !_Animator.GetBool(_AnimationHash))
        {
            MoneyManager.Instance.AddMoney(Value);
            _Animator.SetBool(_AnimationHash, true);
        }
    }
    private void Disable()
    {
        gameObject.SetActive(false);
    }
}
