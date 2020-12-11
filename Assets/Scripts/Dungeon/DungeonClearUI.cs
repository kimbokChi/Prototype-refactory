using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonClearUI : MonoBehaviour
{
    [SerializeField] private Item[] UnlockItems;

    [SerializeField] private Image[] ItemBoxes;
    
    private void Awake()
    {
        for (int i = 0; i < UnlockItems.Length; ++i)
        {
            ItemBoxes[i].sprite = UnlockItems[i].Sprite;
        }
        ItemLibrary.Instance.ItemUnlock(UnlockItems);
    }
}
