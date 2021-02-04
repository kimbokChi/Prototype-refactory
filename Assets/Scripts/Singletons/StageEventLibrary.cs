using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NotifyMessage
{
    StageClear, StageEnter
}

public class StageEventLibrary : Singleton<StageEventLibrary>
{
    [Range(0f, 1f)]
    [SerializeField] private float _ItemBoxProbab;

    [Header("ItemBox Section")]
    [SerializeField] private ItemBoxHolder ItemBox;
    [SerializeField] private ItemBoxSprite WoodenSprite;
    [SerializeField] private ItemBoxSprite GoldenSprite;

    [Header("NPC Section")]
    [SerializeField] private GameObject  PeddlerNPC;
                     private GameObject _PeddlerNPC;

    [SerializeField] private Animator InventoryButton;

    public event System.Action StageClearEvent;
    public event System.Action StageEnterEvent;

    private void Awake()
    {
        StageClearEvent += () =>
        {
            if (Random.value < _ItemBoxProbab)
            {
                CreateItemBox();
            }
            else
            {
                OnEnablePeddlerNPC();
            }
        };
        StageClearEvent += () => SetActiveInventoryButton(true);

        StageEnterEvent += () =>
        {
            if (Castle.Instance.PlayerFloor.FloorIndex != 1)
            {

                SetActiveInventoryButton(false);
            }
        };
        StageEnterEvent += OnDisablePeddlerNPC;
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
        Vector2 createPoint = Castle.Instance.GetMovePoint(UnitizedPos.MID);

        var boxSprite = WoodenSprite;
        var boxContainItem = ItemLibrary.Instance.GetRandomItem();

        if (boxContainItem != null)
        {
            switch (boxContainItem.Rating)
            {
                case ItemRating.Common:
                case ItemRating.Rare:
                    boxSprite = WoodenSprite;
                    break;

                case ItemRating.Epic:
                case ItemRating.Legendary:
                    boxSprite = GoldenSprite;
                    break;
            }
        }
        var box = Instantiate(ItemBox, createPoint, Quaternion.identity);
            box.HoldingItemBox.Init(boxContainItem, boxSprite);
    }

    private void OnEnablePeddlerNPC()
    {
        if (_PeddlerNPC == null)
        {
            _PeddlerNPC = Instantiate(PeddlerNPC);
        }
        _PeddlerNPC.SetActive(true);
        _PeddlerNPC.transform.position = (Vector2)Camera.main.transform.position;
    }
    private void OnDisablePeddlerNPC()
    {
        _PeddlerNPC?.SetActive(false);
    }

    private Vector2 RandomRoomPoint()
    {
        // 0 or 3 or 6
        int floorIndex = Random.Range(0, 3) * 3;

        Vector2 createPointMin = Castle.Instance.GetMovePoint((UnitizedPos)floorIndex);
        Vector2 createPointMax = Castle.Instance.GetMovePoint((UnitizedPos)floorIndex + 2);

        return new Vector2(Random.Range(createPointMin.x, createPointMax.x), createPointMin.y + 0.5f);
    }

    private void SetActiveInventoryButton(bool isActive)
    {
        int controlKey = InventoryButton.GetParameter(0).nameHash;

        InventoryButton.SetInteger(controlKey, isActive ? 1 : 0);
    }
}
