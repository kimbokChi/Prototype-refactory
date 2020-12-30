using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemInfoShower : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool IsEnable = true;
    public Action OnPopupEvent;

    [SerializeField] private DIRECTION9 PopupPivot;

    public void SetPopupPivot(DIRECTION9 pivot)
    {
        PopupPivot = pivot;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (IsEnable)
        {
            OnPopupEvent?.Invoke();

            ItemInfoPopup.Instance.ShowPopup(PopupPivot, transform.position);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (IsEnable)
        {
            ItemInfoPopup.Instance.ClosePopup();
        }
    }
}
