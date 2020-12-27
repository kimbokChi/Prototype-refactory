using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfoPopup : Singleton<ItemInfoPopup>
{
    [SerializeField] private GameObject    _PopupObject;
    [SerializeField] private RectTransform _RectTransform;

    public void ShowPopup(DIRECTION9 pivot, Vector2 position)
    {
        switch (pivot)
        {
            case DIRECTION9.TOP_LEFT:
                _RectTransform.pivot = new Vector2(0.0f, 1.0f);
                break;
            case DIRECTION9.TOP:
                _RectTransform.pivot = new Vector2(0.5f, 1.0f);
                break;
            case DIRECTION9.TOP_RIGHT:
                _RectTransform.pivot = new Vector2(1.0f, 1.0f);
                break;
            case DIRECTION9.MID_LEFT:
                _RectTransform.pivot = new Vector2(0.0f, 0.5f);
                break;
            case DIRECTION9.MID:
                _RectTransform.pivot = new Vector2(0.5f, 0.5f);
                break;
            case DIRECTION9.MID_RIGHT:
                _RectTransform.pivot = new Vector2(1.0f, 0.5f);
                break;
            case DIRECTION9.BOT_LEFT:
                _RectTransform.pivot = new Vector2(0.0f, 0.0f);
                break;
            case DIRECTION9.BOT:
                _RectTransform.pivot = new Vector2(0.5f, 0.0f);
                break;
            case DIRECTION9.BOT_RIGHT:
                _RectTransform.pivot = new Vector2(1.0f, 0.0f);
                break;
            default:
                break;
        }
        _PopupObject.SetActive(true);

        _RectTransform.position = position;
    }

    public void ClosePopup()
    {
        _PopupObject.SetActive(false);
    }
}
