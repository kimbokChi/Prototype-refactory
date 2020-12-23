using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonSelectUI : MonoBehaviour
{
    public Transform mContent;
    public Button[] mSelections;

    private DungeonSelection mDungeon;

    private void OnDisable()
    {
        transform.localScale = Vector2.zero;
    }

    void Awake()
    {
        mSelections = mContent.GetComponentsInChildren<Button>();
    }

    public void SelectDungeon(Button button)
    {
        Debug.Assert(button.TryGetComponent(out mDungeon));
    }

    public void DungeonEnter()
    {
        Debug.Log("Dungeon Enter : " + mDungeon.Name);
        mDungeon?.DungeonEnter();
    }
}
