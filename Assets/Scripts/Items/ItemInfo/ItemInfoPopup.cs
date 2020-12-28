using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoPopup : Singleton<ItemInfoPopup>
{
    [SerializeField] private ItemKeywordContainer _KeywordContainer;
    [Space()]
    [SerializeField] private Image _ItemImage;
    [SerializeField] private GameObject    _PopupObject;
    [SerializeField] private RectTransform _RectTransform;

    [Header("ItemData Section")]
    [SerializeField] private TMPro.TextMeshProUGUI _NameText;
    [SerializeField] private TMPro.TextMeshProUGUI _RatingText;
    [SerializeField] private TMPro.TextMeshProUGUI _ATKText;
    [SerializeField] private TMPro.TextMeshProUGUI _DPSText;

    [Header("WeaponData Section")]
    [SerializeField] private TMPro.TextMeshProUGUI _WeaponInfoText;
    [SerializeField] private TMPro.TextMeshProUGUI _WeaponAblityText;

    [Header("AccessoryData Section")]
    [SerializeField] private TMPro.TextMeshProUGUI _AccessoryInfoText;
    [SerializeField] private TMPro.TextMeshProUGUI _AccessoryAblityText;

    private ItemInfo _ItemInfo;

    public void SetPopup(ItemInfo info)
    {
        _ItemInfo = info;

        _ItemImage.sprite = info.ItemSprite;

        _NameText.text = info.ItemName;
        _NameText.color = info.TextColor;
        _RatingText.text = info.ItemRatingText;
        _RatingText.color = info.TextColor;

        _ATKText.text = $"ATK : {info.ATK}";
        _DPSText.text = $"DPS : {info.DPS}";

        _WeaponInfoText.text = info.WeaponText;
        _WeaponAblityText.text = info.WeaponAblity;

        _AccessoryInfoText.text = info.AccessoryText;
        _AccessoryAblityText.text = info.AccessoryAblity;
    }

    public void ShowPopup(DIRECTION9 pivot, Vector2 position)
    {
        TPOSITION3 keywordPosition = TPOSITION3.LEFT;

        switch (pivot)
        {
            case DIRECTION9.TOP_LEFT:
                keywordPosition = TPOSITION3.LEFT;
                _RectTransform.pivot = new Vector2(0.0f, 1.0f);
                break;
            case DIRECTION9.TOP:
                keywordPosition = TPOSITION3.MID;
                _RectTransform.pivot = new Vector2(0.5f, 1.0f);
                break;
            case DIRECTION9.TOP_RIGHT:
                keywordPosition = TPOSITION3.RIGHT;
                _RectTransform.pivot = new Vector2(1.0f, 1.0f);
                break;
            case DIRECTION9.MID_LEFT:
                keywordPosition = TPOSITION3.LEFT;
                _RectTransform.pivot = new Vector2(0.0f, 0.5f);
                break;
            case DIRECTION9.MID:
                keywordPosition = TPOSITION3.MID;
                _RectTransform.pivot = new Vector2(0.5f, 0.5f);
                break;
            case DIRECTION9.MID_RIGHT:
                keywordPosition = TPOSITION3.RIGHT;
                _RectTransform.pivot = new Vector2(1.0f, 0.5f);
                break;
            case DIRECTION9.BOT_LEFT:
                keywordPosition = TPOSITION3.LEFT;
                _RectTransform.pivot = new Vector2(0.0f, 0.0f);
                break;
            case DIRECTION9.BOT:
                keywordPosition = TPOSITION3.MID;
                _RectTransform.pivot = new Vector2(0.5f, 0.0f);
                break;
            case DIRECTION9.BOT_RIGHT:
                keywordPosition = TPOSITION3.RIGHT;
                _RectTransform.pivot = new Vector2(1.0f, 0.0f);
                break;
            default:
                break;
        }
        _PopupObject.SetActive(true);
        _KeywordContainer.ShowKeyword(keywordPosition, _ItemInfo.UsageKeywords);

        _RectTransform.position = position;
    }

    public void ClosePopup()
    {
        _PopupObject.SetActive(false);
        _KeywordContainer.gameObject.SetActive(false);
    }
}
