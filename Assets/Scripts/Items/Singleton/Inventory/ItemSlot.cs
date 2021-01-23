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
public class ItemSlot : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
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

            Shower.SetPopupEnable(false);
        }
        else
        {
            Shower.IsEnable = true;
            item.OnEquipThis(mSlotType);

            mImage.sprite = item.Sprite;
        }
        if (mSlotType == SlotType.Weapon)
        {
            if (item != null)
            {
                if (item.IsNeedAttackBtn) {
                    AttackButtonController.Instance.ShowButton();
                }
                else {
                    AttackButtonController.Instance.HideButton();
                }
            }
            else {
                AttackButtonController.Instance.HideButton();
            }
        }
        ItemStateSaver.Instance.SaveSlotItem(mSlotType, mContainItem, 0);
    }

    public void ItemSwapFingerAndSlot()
    {
        var containItem = mContainItem;
        Item fingerItem = Finger.Instance.CarryItem;

        if (fingerItem != null)
        {
            ItemInfoPopup.Instance.SetPopup(fingerItem.GetItemInfo);

            Shower.SetPopupEnable(true);
        }
        SetItem(fingerItem);
                Finger.Instance.CarryItem = containItem;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor || 
            Application.platform == RuntimePlatform.WindowsPlayer)
        {
            ItemSwapFingerAndSlot();
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began) {
                    ItemSwapFingerAndSlot();
                }
            }
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Ended) {
                    ItemSwapFingerAndSlot();
                }
            }
        }
    }
}
