using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Path;
using UnityEngine;

public enum NotifyMessage
{
    StageClear, StageEnter
}

public class StageEventLibrary : Singleton<StageEventLibrary>
{
    [SerializeField]
    private GameObject ItemBox;
    [SerializeField]
    private GameObject DungeonNPC;

    [SerializeField]
    private Animator InventoryButton;

    public event System.Action StageClearEvent;
    public event System.Action StageEnterEvent;

    private void Awake()
    {
        StageClearEvent += CreateItemBox;
        StageClearEvent += CreateNPC;
        StageClearEvent += () => SetActiveInventoryButton(true);

        StageEnterEvent += () => SetActiveInventoryButton(false);
    }

    public void NotifyEvent(NotifyMessage message)
    {
        switch (message)
        {
            case NotifyMessage.StageClear:
                StageClearEvent?.Invoke();
                break;

            case NotifyMessage.StageEnter:
                StageEnterEvent?.Invoke();
                break;
        }
    }

    private void CreateItemBox()
    {
        Vector2 createPoint = Castle.Instance.GetMovePoint(DIRECTION9.MID);

        Instantiate(ItemBox, createPoint, Quaternion.identity);
    }

    private void CreateNPC()
    {
    }

    private Vector2 RandomRoomPoint()
    {
        // 0 or 3 or 6
        int floorIndex = Random.Range(0, 3) * 3;

        Vector2 createPointMin = Castle.Instance.GetMovePoint((DIRECTION9)floorIndex);
        Vector2 createPointMax = Castle.Instance.GetMovePoint((DIRECTION9)floorIndex + 2);

        return new Vector2(Random.Range(createPointMin.x, createPointMax.x), createPointMin.y + 0.5f);
    }

    private void SetActiveInventoryButton(bool isActive)
    {
        int controlKey = InventoryButton.GetParameter(0).nameHash;

        InventoryButton.SetInteger(controlKey, isActive ? 1 : 0);
    }
}
