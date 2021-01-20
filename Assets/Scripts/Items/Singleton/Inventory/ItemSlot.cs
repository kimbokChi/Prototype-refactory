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

    private Coroutine _Coroutine;
    private bool _IsWaitForTouchOver = false;

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
        _Coroutine = new Coroutine(this);
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
        _Coroutine.StartRoutine(WaitForPressInput());
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Finger.Instance.CarryItem != null)
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    _Coroutine.StartRoutine(WaitForInputOver());
                    break;

                case RuntimePlatform.Android:
                    _IsWaitForTouchOver = true;
                    break;
            }
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (_IsWaitForTouchOver)
        {
            if (Input.touchCount > 0) {
                if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    _IsWaitForTouchOver = false;

                    ItemSwapFingerAndSlot();
                }
            }
        }
        _Coroutine.StopRoutine();
    }
    private IEnumerator WaitForPressInput()
    {
        yield return new WaitForSeconds(0.4f);
        ItemSwapFingerAndSlot();

        _Coroutine.Finish();
    }
    private IEnumerator WaitForInputOver()
    {
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        ItemSwapFingerAndSlot();

        _Coroutine.Finish();
    }
}
