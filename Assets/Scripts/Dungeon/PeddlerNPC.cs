﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeddlerNPC : MonoBehaviour
{
    [Header("Item Section")]
    [Range(1f, 2f)]
    [SerializeField] private float ItemCostScalling;
    [SerializeField] private SpriteRenderer ShowItemRenderer;
    [SerializeField] private Animator ShowItemAnimator;
    [SerializeField] private GameObject InteractionUI;

    [Header("Message Section")]
    [SerializeField] private TMPro.TextMeshProUGUI ItemCostText;
    [SerializeField] private TMPro.TextMeshProUGUI ItemNameText;
    [SerializeField] private TMPro.TextMeshProUGUI  MessageText;

    private int _DropItemControlKey;
    private int _ItemCost;
    private Item _ContainItem;

    private void OnEnable()
    {
        var list = ItemLibrary.Instance.GetUnlockedItemListForTest();

        _ContainItem = list[Random.Range(0, list.Count)];

        ShowItemRenderer.sprite = _ContainItem.Sprite;
        _ItemCost = (int)System.Math.Ceiling(_ContainItem.Cost * ItemCostScalling);

        ItemCostText.gameObject.SetActive(true);
        ItemNameText.gameObject.SetActive(true);

        ItemCostText.text = _ItemCost.ToString();
        ItemNameText.text = _ContainItem.NameKR;

        if (_DropItemControlKey == default)
        {
            _DropItemControlKey = ShowItemAnimator.GetParameter(0).nameHash;
        }
        MessageText.text = "구매확인";
    }
    private void OnDisable()
    {
        ShowItemAnimator.SetBool(_DropItemControlKey, false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) InteractionUI.SetActive(true);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            InteractionUI.SetActive(false);
        }
    }

    public void Purchace()
    {
        if (MoneyManager.Instance.SubtractMoney(_ItemCost))
        {
            var item = Instantiate(_ContainItem);
                item.transform.position = new Vector2(-10f, 0f);

            Inventory.Instance.AddItem(item);
            item.transform.parent = ItemStateSaver.Instance.transform;

            ShowItemAnimator.SetBool(_DropItemControlKey, true);

            ItemCostText.gameObject.SetActive(false);
            ItemNameText.gameObject.SetActive(false);

            MessageText.text = "구매완료!";
        }
    }
}
