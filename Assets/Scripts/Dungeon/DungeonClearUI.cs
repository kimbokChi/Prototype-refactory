using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DungeonClearUI : MonoBehaviour
{
    public Action KillEnemy;

    [SerializeField] private TMPro.TextMeshProUGUI KillCount;
    [SerializeField] private TMPro.TextMeshProUGUI ClearTime;

    [SerializeField] private Item [] UnlockItems;
    [SerializeField] private Image[] ItemBoxes;

    private void Awake()
    {
        for (int i = 0; i < UnlockItems.Length; ++i)
        {
            ItemBoxes[i].sprite = UnlockItems[i].Sprite;
        }
        ItemLibrary.Instance.ItemUnlock(UnlockItems);

        int clearSec = Mathf.FloorToInt(GameLoger.Instance.ElapsedTime % 60f);
        int clearMin = Mathf.FloorToInt(GameLoger.Instance.ElapsedTime / 60f);

        ClearTime.text = $"{clearMin:D2} : {clearSec:D2}";
        KillCount.text = $"{GameLoger.Instance.KillCount:D3} 마리";
    }

    public void Close()
    {
        MainCamera.Instance.Fade(2.25f, FadeType.In, () => SceneManager.LoadScene(0));
    }
}
