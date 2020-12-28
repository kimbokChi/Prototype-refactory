using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Keyword
{
    Strike, Charge, Entry
}
[Serializable]
public struct KeywordBlock
{
    public Keyword Keyword;
    public GameObject KeywordInfoUI;
}

public class ItemKeywordContainer : MonoBehaviour
{
    [SerializeField] private RectTransform   _RectTransform;
    [SerializeField] private GridLayoutGroup _GridLayoutGroup;
    [SerializeField] private KeywordBlock[]  _KeywordBlocks;

    [Header("GridLayoutGroup")]
    [SerializeField] private RuntimePresets.Preset _GLGHorizontal;
    [SerializeField] private RuntimePresets.Preset _GLGVertical;

    [Header("RectTransform")]
    [SerializeField] private RuntimePresets.Preset _RTLeft;
    [SerializeField] private RuntimePresets.Preset _RTRight;
    [SerializeField] private RuntimePresets.Preset _RTUp;

    private Dictionary<Keyword, GameObject> _KeywordBlockCollection;
    private bool _IsAlreadyInit = false;

    private void OnEnable()
    {
        if (!_IsAlreadyInit)
        {
            _KeywordBlockCollection = new Dictionary<Keyword, GameObject>();

            for (int i = 0; i < _KeywordBlocks.Length; ++i)
            {
                _KeywordBlockCollection.Add
                    (_KeywordBlocks[i].Keyword, _KeywordBlocks[i].KeywordInfoUI);

                Debug.Log("Init : " + _KeywordBlocks[i].Keyword);
            }
            _IsAlreadyInit = true;
        }
    }

    private void OnDisable()
    {
        foreach (var keywordBlock in _KeywordBlockCollection)
        {

            keywordBlock.Value.SetActive(false);
        }
    }

    public void ShowKeyword(TPOSITION3 position, params Keyword[] keywords)
    {
        gameObject.SetActive(true);

        var RTTargetPreset = _RTLeft;

        switch (position)
        {
            case TPOSITION3.LEFT:
                RTTargetPreset = _RTLeft;
                break;
            case TPOSITION3.MID:
                RTTargetPreset = _RTUp;
                break;
            case TPOSITION3.RIGHT:
                RTTargetPreset = _RTRight;
                break;
        }
        if (RTTargetPreset.CanBeAppliedTo(_RectTransform))
        {
            RTTargetPreset.ApplyTo(_RectTransform);
        }
        if (position == TPOSITION3.MID)
        {
            if (_GLGHorizontal.CanBeAppliedTo(_GridLayoutGroup))
            {
                _GLGHorizontal.ApplyTo(_GridLayoutGroup);
            }
        }
        else
        {
            if (_GLGVertical.CanBeAppliedTo(_GridLayoutGroup))
            {
                _GLGVertical.ApplyTo(_GridLayoutGroup);
            }
        }
        for (int i = 0; i < keywords.Length; ++i)
        {
            Debug.Log(keywords[i]);
            _KeywordBlockCollection[keywords[i]].SetActive(true);
        }
    }
}
