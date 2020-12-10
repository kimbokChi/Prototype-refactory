using UnityEngine;
using UnityEngine.UI;

public class ItemCollection : MonoBehaviour
{
    [Header("Unlocked Section")]
    [SerializeField] private  Item[] UnlockedItems;
    [SerializeField] private Image[] UnlockedItemImages;

    [Header("Locked Section")]
    [SerializeField] private  Item[] LockedItems;
    [SerializeField] private Image[] LockedItemImages;

    private void Awake()
    {
        for (int i = 0; i < LockedItems.Length; i++)
        {
            LockedItemImages[i].sprite = LockedItems[i].Sprite;
        }
        for (int i = 0; i < UnlockedItems.Length; i++)
        {
            UnlockedItemImages[i].sprite = UnlockedItems[i].Sprite;
        }
    }
}
