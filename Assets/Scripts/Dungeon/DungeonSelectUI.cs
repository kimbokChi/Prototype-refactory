using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonSelectUI : MonoBehaviour
{
    public Transform mContent;
    public Button[] mSelections;

    private DungeonSelection mDungeon;

    void Awake()
    {
        mSelections = mContent.GetComponentsInChildren<Button>();
    }

    public void SelectDungeon(Button button)
    {
        foreach (Button Child in mSelections)
            Child.image.color = new Color(1f, 1f, 1f, 0.5f);

        button.image.color = new Color(1f, 0, 0, 0.5f);

        Debug.Assert(button.TryGetComponent(out mDungeon));
    }

    public void DungeonEnter()
    {
        mDungeon?.DungeonEnter();
    }
}
