using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemCollection : MonoBehaviour
{
    [Header("Unlocked Section")]
    [SerializeField] private   Item[] UnlockedItems;
    [SerializeField] private Button[] UnlockedItemButtons;

    [Header("Locked Section")]
    [SerializeField] private   Item[] LockedItems;
    [SerializeField] private Button[] LockedItemButtons;

    private void Awake()
    {
        for (int i = 0; i < LockedItems.Length; i++)
        {
            LockedItemButtons[i].image.sprite = LockedItems[i].Sprite;
        }
        for (int i = 0; i < UnlockedItems.Length; i++)
        {
            UnlockedItemButtons[i].image.sprite = UnlockedItems[i].Sprite;
        }
    }
}
