using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkTheSpearman : MonoBehaviour, IObject, ICombatable, IAnimEventReceiver
{
    [Header("Spear Info")]
    [SerializeField] private Arrow Spear;
    [SerializeField] private float ShootSpeed;
    [SerializeField] private Vector2 ShootPos;

    [Header("Ability")]
    [SerializeField] private AbilityTable AbilityTable;
    [SerializeField] private EnemyAnimator EnemyAnimator;

    private AttackPeriod _AttackPeriod;

    private IEnumerator _EMove;

    private Player mPlayer;

    public void AnimationPlayOver(AnimState anim)
    {
        switch (anim)
        {
            case AnimState.Attack:
                
                break;

            case AnimState.Damaged:
                EnemyAnimator.ChangeState(AnimState.Idle);
                break;

            case AnimState.Death:
                gameObject.SetActive(false);
                break;

            case AnimState.AttackBegin:

                break;
        }
    }

    public void Damaged(float damage, GameObject attacker)
    {
        EffectLibrary.Instance.UsingEffect(EffectKind.EnemyDmgEffect, transform.position);

        if ((AbilityTable.Table[Ability.CurHealth] -= damage) <= 0)
        {
            _AttackPeriod.StopPeriod();

            EnemyAnimator.ChangeState(AnimState.Death);
            HealthBarPool.Instance.UnUsingHealthBar(transform);
        }
    }

    public void IInit()
    {
        EnemyAnimator.Init();
        HealthBarPool.Instance.UsingHealthBar(-1f, transform, AbilityTable);

        _AttackPeriod = new AttackPeriod(AbilityTable);

        _AttackPeriod.SetAction(Period.Attack, () =>
        {
            if (_EMove != null)
            {
                StopCoroutine(_EMove);
                _EMove = null;
            }

            EnemyAnimator.ChangeState(AnimState.AttackBegin);
        });
        Spear = Instantiate(Spear);
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

    public GameObject ThisObject() => gameObject;
    public AbilityTable GetAbility => AbilityTable;
    public bool IsActive()
    {
        return gameObject.activeSelf;
    }
    public void CastBuff(BUFF buffType, IEnumerator castedBuff)
    {
        StartCoroutine(castedBuff);
    }
}
