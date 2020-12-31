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
public class ItemSlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
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

    public void OnPointerDown(PointerEventData eventData)
    {
        _Coroutine.StartRoutine(WaitPress());
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        // OnPointerUp은 이전에 Down입력이 있어야지 작동해서 쓰기가 좀 힘드네여
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        _Coroutine.StartRoutine(WaitInputOver());
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        _Coroutine.StopRoutine();
    }
    private IEnumerator WaitPress()
    {
        yield return new WaitForSeconds(0.6f);
        Select();

        _Coroutine.Finish();
    }
    private IEnumerator WaitInputOver()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.WindowsEditor:
                yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
                break;

            case RuntimePlatform.Android:
                yield return new WaitUntil(() => Input.touchCount == 0);
                break;
        }
        Select();

        _Coroutine.Finish();
    }
}
