using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    [SerializeField]
    private EffectKind EffectKind;

    public void LifeTimeOver() {
        EffectLibrary.Instance.UnUsingEffect(EffectKind, this);
    }
}
