using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct ItemShowBlock
{
    public Text ItemNameText;
    public Text ItemCostText;

    public Image ItemImage;
}

public class ItemStore : MonoBehaviour
{
    [SerializeField] private RegisteredItem RegisteredItem;
    [SerializeField] private ItemShowBlock[] ItemBlocks;

    private bool _IsAlreadyInit = false;

    private void Start()
    {
        if (!_IsAlreadyInit)
        {
            var list = ItemLibrary.Instance.GetUnlockedItemListForTest();

            for (int i = 0; i < 3; i++)
            {
                int  index = Random.Range(0, list.Count);
                int rating = (int)list[index].Rating;

                ItemBlocks[i].ItemImage.sprite = list[index].Sprite;

                ItemBlocks[i].ItemNameText.text = list[index].GetType().ToString();
                ItemBlocks[i].ItemCostText.text = Random.Range(8 + rating, 8 + 3 * rating).ToString();

                list.RemoveAt(index);
            }
            _IsAlreadyInit = true;
        }
    }
}
