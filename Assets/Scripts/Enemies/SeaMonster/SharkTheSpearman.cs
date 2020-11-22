using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkTheSpearman : MonoBehaviour, IObject, ICombatable, IAnimEventReceiver
{
    [SerializeField] private AbilityTable AbilityTable;
    [SerializeField] private EnemyAnimator EnemyAnimator;

    private Player mPlayer;

    public void AnimationPlayOver(AnimState anim)
    {
        // ToDo
    }

    public void CastBuff(BUFF buffType, IEnumerator castedBuff)
    {
        StartCoroutine(castedBuff);
    }

    public void Damaged(float damage, GameObject attacker)
    {
        
    }

    public void IInit()
    {
        
    }

    public void IUpdate()
    {

    }

    public void PlayerEnter(MESSAGE message, Player enterPlayer)
    {
        if (AbilityTable.CanRecognize(message))
        {

            mPlayer = enterPlayer;
        }
    }

    public void PlayerExit(MESSAGE message)
    {
        if (AbilityTable.CantRecognize(message)) {

            mPlayer = null;
        }
    }

    public AbilityTable GetAbility => AbilityTable;
    public GameObject ThisObject() => gameObject;
    public bool IsActive()
    {
        return gameObject.activeSelf;
    }
}
