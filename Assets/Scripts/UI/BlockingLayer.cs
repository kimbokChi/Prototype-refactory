using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlockingLayer : MonoBehaviour, IPointerDownHandler
{
    private GameObject _User;

    public void SetActive(bool isActive, GameObject user)
    {
        if (isActive)
        {
            _User = user;
        }
        else
        {
            _User = null;
        }
        gameObject.SetActive(isActive);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        _User?.SetActive(false);
        _User = null;

        gameObject.SetActive(false);
    }
}
