using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupNPC : NPC
{
    [SerializeField] private SubscribableButton _InteractionBtn;

    [SerializeField, Header("PopupNPC Property")]
    private GameObject _Popup;

    [SerializeField]
    private bool _WaitForEffectEnd;

    private void Start()
    {
        _InteractionBtn.ButtonAction += state =>
        {
            if (state == ButtonState.Down) {
                Interaction();
            }
        };
    }

    public override void Interaction()
    {
        if (_WaitForEffectEnd)
        {
            EffectLibrary.Instance.UsingEffect(EffectKind.Twinkle, transform.position, () =>
            {
                _Popup.SetActive(true);
            });
        }
        else
        {
            EffectLibrary.Instance.UsingEffect(EffectKind.Twinkle, transform.position);
            _Popup.SetActive(true);
        }
    }
}
