using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemShowBlock : MonoBehaviour
{
    [SerializeField] private ItemStore Store;

    [SerializeField] private Text ItemNameText;
    [SerializeField] private Text ItemCostText;

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
        int rating = (int)blockItem.Rating + 8;

        ContainItem = blockItem;
        ItemCost = Random.Range(rating, rating * 3);

        ItemNameText.text = blockItem.GetType().ToString();
        ItemCostText.text = ItemCost.ToString();

        ItemImage.sprite = blockItem.Sprite;
    }
}
