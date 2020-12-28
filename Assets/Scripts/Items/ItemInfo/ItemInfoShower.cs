using UnityEngine;
using UnityEngine.EventSystems;

public class ItemInfoShower : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private DIRECTION9 PopupPivot;

    public void OnPointerEnter(PointerEventData eventData)
    {
        ItemInfoPopup.Instance.ShowPopup(PopupPivot, transform.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ItemInfoPopup.Instance.ClosePopup();
    }
}
