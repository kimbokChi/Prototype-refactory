using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NPC : MonoBehaviour
{
    [SerializeField] protected string _Key;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            if (GameLoger.Instance.UsingVJoystick)
            {
                NPCInteractionManager.Instance.VJoystick_SetCoreBtnMode(CoreBtnMode.InteractionOrder);
            }
            PlayerEvent(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            if (GameLoger.Instance.UsingVJoystick)
            {
                NPCInteractionManager.Instance.VJoystick_SetCoreBtnMode(CoreBtnMode.AttackOrder);
            }
            PlayerEvent(false);
        }
    }

    public abstract void Interaction();
    public virtual void PlayerEvent(bool enter)
    {
        NPCInteractionManager.Instance.SetActive(_Key, enter);
    }
}