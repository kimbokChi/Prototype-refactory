using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class DungeonSelection : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private DungeonSelectionInfo _SelectionInfo;

    [Header("Selection Parameter")]
    [SerializeField] private Image _Image;
    [SerializeField] private TMPro.TextMeshProUGUI _Title;
    [SerializeField] private TMPro.TextMeshProUGUI _Comment;

    public event Action<DungeonSelection> SelectedEvent;
    public string Name => _SelectionInfo.AttachedSceneName;

    public void Init(bool isLocked)
    {
        if (isLocked)
        {
            _Image.sprite = _SelectionInfo.LockedSprite;
            _Title.text   = _SelectionInfo.LockedTitle;
            _Comment.text = _SelectionInfo.LockedComment;
        }
        else
        {
            _Image.sprite = _SelectionInfo.UnLockedSprite;
            _Title.text   = _SelectionInfo.UnLockedTitle;
            _Comment.text = _SelectionInfo.UnLockedComment;
        }
    }

    public void DungeonEnter()
    {
        MainCamera.Instance.Fade(1.75f, FadeType.In, () => 
        {
            SceneManager.LoadScene(_SelectionInfo.AttachedSceneName);
        });
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SelectedEvent?.Invoke(this);
    }
}