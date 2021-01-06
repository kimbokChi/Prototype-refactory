using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttackButton : MonoBehaviour, IPointerDownHandler
{
    public event Action IntractEvent;

    public void OnPointerDown(PointerEventData eventData)
    {
        IntractEvent?.Invoke();
    }
}
