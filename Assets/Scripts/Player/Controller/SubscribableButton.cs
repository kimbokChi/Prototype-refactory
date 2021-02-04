using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum ButtonState { Down, Up }

public class SubscribableButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public event Action<ButtonState> ButtonAction;

    public void OnPointerDown(PointerEventData eventData)
    {
        ButtonAction?.Invoke(ButtonState.Down);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        ButtonAction?.Invoke(ButtonState.Up);
    }
}
