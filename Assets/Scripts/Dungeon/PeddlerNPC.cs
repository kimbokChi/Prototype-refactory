using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeddlerNPC : MonoBehaviour
{
    [SerializeField] private Canvas WorldSpaceCanvas;

    [Header("Item Section")]
    [SerializeField] private SpriteRenderer ShowItemRenderer;
    [Range(1f, 2f)]
    [SerializeField] private float ItemCostScalling;
    [SerializeField] private TMPro.TextMeshProUGUI ItemCostText;
    [SerializeField] private TMPro.TextMeshProUGUI ItemNameText;

    private int _ItemCost;
    private Item _ContainItem;

    private void OnEnable()
    {
        var list = ItemLibrary.Instance.GetUnlockedItemListForTest();

        _ContainItem = list[Random.Range(0, list.Count)];

        ShowItemRenderer.sprite = _ContainItem.Sprite;
        _ItemCost = (int)System.Math.Ceiling(_ContainItem.Cost * ItemCostScalling);

        ItemCostText.text = _ItemCost.ToString();
        ItemNameText.text = _ContainItem.NameKR;
    }
}
