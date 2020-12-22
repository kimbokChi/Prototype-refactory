using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemShowBlock : MonoBehaviour
{
    [SerializeField] private ItemStore Store;

    [SerializeField] private TMPro.TextMeshProUGUI ItemNameText;
    [SerializeField] private TMPro.TextMeshProUGUI ItemCostText;

    [SerializeField] private Image ItemImage;

    public int ItemCost
    {
        get;
        private set;
    }
    public Item ContainItem
    {
        get;
        private set;
    }

    public void Selected()
    {
        Store.BlockSelect(this);
    }

    public void SetBlock(Item blockItem)
    {
        ContainItem = blockItem;
        ItemCost = blockItem.Cost;

        ItemNameText.text = blockItem.NameKR;
        ItemCostText.text = ItemCost.ToString();

        ItemImage.sprite = blockItem.Sprite;
    }
}
