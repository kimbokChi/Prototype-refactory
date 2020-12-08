using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : MonoBehaviour
{
    [SerializeField] private DropItem DropItem;
    [SerializeField] private Animator Animator;

    private void OnEnable()
    {
        DropItem.Init(ItemLibrary.Instance.GetRandomItem());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) {

            Animator.enabled = true;
        }
    }
}
