using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SubscribableButton : MonoBehaviour, IPointerDownHandler
{
    public event Action ButtonDown;

    public void OnPointerDown(PointerEventData eventData)
    {
        ButtonDown?.Invoke();
    }
}
