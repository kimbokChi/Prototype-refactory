﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : MonoBehaviour
{
    [SerializeField] private DropItem DropItem;

    [Header("Animation")]
    [SerializeField] private Animator Animator;
    [SerializeField] private Sprite ChestOpenSprite;

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

    private void ChestOpen()
    {
        if (TryGetComponent(out SpriteRenderer renderer)) {

            renderer.sprite = ChestOpenSprite;

            Vector2 twinklePoint = transform.position + new Vector3(0f, 0.4f, 0f);
            EffectLibrary.Instance.UsingEffect(EffectKind.Twinkle, twinklePoint);
        }
    }
}
