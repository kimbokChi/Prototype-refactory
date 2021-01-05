using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonSelectUI : MonoBehaviour
{
    [SerializeField] private GameObject _SelectedStamp;
    [SerializeField] private DungeonSelection[] _Selections;

    private DungeonSelection _SelectedDungeon;
    private bool _IsAlreadyInit = false;

    private void OnDisable()
    {
        transform.localScale = Vector2.zero;
    }

    private void Start()
    {
        if (!_IsAlreadyInit)
        {
            _SelectedStamp.SetActive(false);

            for (int i = 0; i < _Selections.Length; i++)
            {
                bool isLocked = i > GameLoger.Instance.UnlockDungeonIndex;

                _Selections[i].Init(isLocked);

                if (!isLocked)
                {
                    _Selections[i].SelectedEvent += selected =>
                    {
                        _SelectedDungeon = selected;

                        _SelectedStamp.SetActive(true);
                        _SelectedStamp.transform.parent = selected.transform;
                        _SelectedStamp.transform.localPosition = new Vector3(0, 45, 0);
                    };
                }
            }
        }
    }
    public void DungeonEnter()
    {
        _SelectedDungeon?.DungeonEnter();
    }
}
