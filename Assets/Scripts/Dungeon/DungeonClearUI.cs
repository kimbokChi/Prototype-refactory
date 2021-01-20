using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DungeonClearUI : MonoBehaviour
{
    [Header("UnlockDungeon")]
    [SerializeField] private int _UnlockDungeonIndex;
    [SerializeField] private DungeonSelectionInfo _UnlockDungeonInfo;

    [Header("UnlockDungeon Para")]
    [SerializeField] private Image _DungeonInfoImage;
    [SerializeField] private TMPro.TextMeshProUGUI _DungeonUnlockMessage;
    [SerializeField] private TMPro.TextMeshProUGUI _DungeonTitle;
    [SerializeField] private TMPro.TextMeshProUGUI _DungeonComment;

    [Header("Parameter")]
    [SerializeField] private TMPro.TextMeshProUGUI KillCount;
    [SerializeField] private TMPro.TextMeshProUGUI ClearTime;

    [Header("UnlockItem")]
    [SerializeField] private Item [] UnlockItems;
    [SerializeField] private Image[] ItemBoxes;

    private void Awake()
    {
        for (int i = 0; i < UnlockItems.Length; ++i)
        {
            ItemBoxes[i].sprite = UnlockItems[i].Sprite;
            ItemStateSaver.Instance.ItemUnlock(UnlockItems[i].ID);
        }
        int clearSec = Mathf.FloorToInt(GameLoger.Instance.ElapsedTime % 60f);
        int clearMin = Mathf.FloorToInt(GameLoger.Instance.ElapsedTime / 60f);

        ClearTime.text = $"{clearMin:D2} : {clearSec:D2}";
        KillCount.text = $"{GameLoger.Instance.KillCount:D3} 마리";


        _DungeonTitle.text       = _UnlockDungeonInfo.UnLockedTitle;
        _DungeonInfoImage.sprite = _UnlockDungeonInfo.UnLockedSprite;
        _DungeonComment.text     = _UnlockDungeonInfo.UnLockedComment;

        // 아직 해금되지 않은 던전일 때에만
        if (_UnlockDungeonIndex > GameLoger.Instance.UnlockDungeonIndex)
        {
            GameLoger.Instance.RecordStageUnlock();

            _DungeonUnlockMessage.text = "해금된 던전";
        }
        else
        {
            _DungeonUnlockMessage.text = "이미 해금됨";
        }
    }

    public void Close()
    {
        MainCamera.Instance.Fade(2.25f, FadeType.In, () =>
        {
            Inventory.Instance.Clear();

            SceneManager.LoadScene(1);
        });
    }
}
