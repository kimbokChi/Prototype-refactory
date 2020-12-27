using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public enum SlotType
{
    Container, Accessory, Weapon
}
public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Sprite EmptySprite;
    [SerializeField] private DIRECTION9 PopupPivot;

    public event Action<Item> ItemChangeEvent;
    public event Action<Item> ItemEquipEvent;

    public  Item  ContainItem => mContainItem;
    private Item mContainItem;
    
    private Image mImage;

    private SlotType mSlotType;

    public void Init(SlotType type)
    {
        mSlotType = type;

        TryGetComponent(out mImage);
    }

    public void SetItem(Item item)
    {
        if (mContainItem != null)
        {
            ItemChangeEvent?.Invoke(mContainItem);

            mContainItem.OffEquipThis(mSlotType);
        }        
        mContainItem = item;

        ItemEquipEvent?.Invoke(mContainItem);

        if (item == null)
        {
            mImage.sprite = EmptySprite;
        }
        else
        {
            item.OnEquipThis(mSlotType);

            mImage.sprite = item.Sprite;
        }
        if (mSlotType == SlotType.Weapon)
        {
            ItemStateSaver.Instance.SaveSlotItem(mSlotType, mContainItem, 0);
        }
    }

    public void Select()
    {
        var containItem = mContainItem;

        SetItem(Finger.Instance.CarryItem);
                Finger.Instance.CarryItem = containItem;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ItemInfoPopup.Instance.ShowPopup(PopupPivot, transform.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ItemInfoPopup.Instance.ClosePopup();
    }
}
