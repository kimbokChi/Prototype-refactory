using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public enum SlotType
{
    Container, Accessory, Weapon
}
public class ItemSlot : MonoBehaviour
{
    [SerializeField] private ItemInfoShower Shower;
    [SerializeField] private Sprite EmptySprite;

    public event Action<Item> ItemChangeEvent;
    public event Action<Item> ItemEquipEvent;

    public  Item  ContainItem => mContainItem;
    private Item mContainItem;
    
    private Image mImage;

    private SlotType mSlotType;

    [ContextMenu("AAA")]
    private void AAA()
    {
        gameObject.TryGetComponent(out Shower);
    }

    public void Init(SlotType type)
    {
        mSlotType = type;

        TryGetComponent(out mImage);

        Shower.OnPopupEvent = () =>
        {
            if (mContainItem != null)
            {
                ItemInfoPopup.Instance.SetPopup(mContainItem.GetItemInfo);
            }
        };
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
            Shower.IsEnable = false;
            mImage.sprite = EmptySprite;
        }
        else
        {
            Shower.IsEnable = true;
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
}
