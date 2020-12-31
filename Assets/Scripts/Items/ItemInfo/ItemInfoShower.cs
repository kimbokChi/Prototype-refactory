using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemInfoShower : MonoBehaviour, IPointerUpHandler
{
    public bool IsEnable = true;
    public Action OnPopupEvent;

    [SerializeField] private DIRECTION9 PopupPivot;

    public void SetPopupPivot(DIRECTION9 pivot)
    {
        PopupPivot = pivot;
    }
    public void SetPopupEnable(bool enablePopup)
    {
        if (enablePopup)
        {
            OnPopupEvent?.Invoke();

            ItemInfoPopup.Instance.ShowPopup(PopupPivot, transform.position);
        }
        else
        {
            ItemInfoPopup.Instance.ClosePopup();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (IsEnable)
        {
            SetPopupEnable(true);
        }
    }
}
