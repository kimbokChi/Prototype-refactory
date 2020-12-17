using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    public event Action OnDisableEvent;

    [SerializeField]
    private EffectKind EffectKind;

    public void LifeTimeOver() 
    {
        EffectLibrary.Instance.UnUsingEffect(EffectKind, this);
    }
    private void OnDisable()
    {
        OnDisableEvent?.Invoke();
        OnDisableEvent = null;
    }
}
