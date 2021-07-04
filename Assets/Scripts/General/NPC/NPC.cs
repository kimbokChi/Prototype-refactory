using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager = NPCInteractionManager;

public abstract class NPC : MonoBehaviour
{
    [SerializeField] protected string _Key;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            PlayerEvent(true);
            Manager.Instance.VJoystick_SetCoreBtnMode(CoreBtnMode.InteractionOrder);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            PlayerEvent(false);
            Manager.Instance.VJoystick_SetCoreBtnMode(CoreBtnMode.AttackOrder);
        }
    }

    public abstract void Interaction();
    public virtual void PlayerEvent(bool enter)
    {
        SetEnable(enter);
        Manager.Instance.SetNowEnableNPC(this, enter);
    }
    public virtual void SetEnable(bool enable)
    {
        Manager.Instance.SetActive(_Key, enable);
    }
}