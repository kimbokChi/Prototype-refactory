using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonClearUI : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI KillCount;
    [SerializeField] private TMPro.TextMeshProUGUI ClearTime;

    [SerializeField] private Item[] UnlockItems;
    [SerializeField] private Image[] ItemBoxes;

    private int   _KillCount = 5;
    private float _ClearTime = 360f;
    
    private void Awake()
    {
        for (int i = 0; i < UnlockItems.Length; ++i)
        {
            ItemBoxes[i].sprite = UnlockItems[i].Sprite;
        }
        ItemLibrary.Instance.ItemUnlock(UnlockItems);

        int clearSec = Mathf.FloorToInt(_ClearTime % 60f);
        int clearMin = Mathf.FloorToInt(_ClearTime / 60f);

        ClearTime.text = $"{clearMin:D2} : {clearSec:D2}";
        KillCount.text = $"{_KillCount:D3} 마리";
    }
}
