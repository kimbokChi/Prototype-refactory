using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tentacle : MonoBehaviour, IObject, ICombatable, IAnimEventReceiver
{
    public Action<Tentacle> DeathrattleAction;

    [SerializeField] private AbilityTable AbilityTable;
    [SerializeField] private EnemyAnimator EnemyAnimator;

    public void AnimationPlayOver(AnimState anim)
    {
        switch (anim)
        {
                // Come Out
            case AnimState.Move:
            case AnimState.Attack:
                EnemyAnimator.ChangeState(AnimState.Idle);
                break;

            case AnimState.Death:
                gameObject.SetActive(false);
                break;
        }
    }

    public void CastBuff(Buff buffType, IEnumerator castedBuff)
    {
        StartCoroutine(castedBuff);
    }

    public void Damaged(float damage, GameObject attacker)
    {
        EffectLibrary.Instance.UsingEffect(EffectKind.EnemyDmgEffect, transform.position);

        if ((AbilityTable.Table[Ability.CurHealth] -= damage) <= 0f)
        {
            DeathrattleAction?.Invoke(this);

            EnemyAnimator.ChangeState(AnimState.Death);

            HealthBarPool.Instance.UnUsingHealthBar(transform);
        }
    }

    public void IInit()
    {
        // HealthBarPool.Instance.UsingHealthBar(-1f, transform, AbilityTable);
    }

    public void IUpdate()
    {

    }

    public void PlayerEnter(MESSAGE message, Player enterPlayer)
    {

    }

    public void PlayerExit(MESSAGE message)
    {

    }

    public AbilityTable GetAbility => AbilityTable;
    public bool IsActive() => gameObject.activeSelf;
    public GameObject ThisObject() => gameObject;
}
