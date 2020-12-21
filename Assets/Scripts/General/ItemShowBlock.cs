using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemShowBlock : MonoBehaviour
{
    [SerializeField] private Text ItemNameText;
    [SerializeField] private Text ItemCostText;

    [SerializeField] private Image ItemImage;

    private Item _ContainItem;

    public void SetBlock(Item blockItem)
    {
        int rating = (int)blockItem.Rating + 8;

        _ContainItem = blockItem;

        ItemNameText.text = blockItem.GetType().ToString();
        ItemCostText.text = Random.Range(rating, rating * 3).ToString();

        ItemImage.sprite = blockItem.Sprite;
    }
}
