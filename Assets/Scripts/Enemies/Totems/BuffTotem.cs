﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffTotem : MonoBehaviour, IObject, ICombatable, IAnimEventReceiver
{
    [Header("Effect Animation Info")]
    [SerializeField] private float AnimLength;
    [SerializeField] private GameObject Anim;

    [Header("BuffTotem Info")]
    [SerializeField] private Animator Animator;
    [SerializeField] private AbilityTable AbilityTable;

    [SerializeField] private Area mSenseArae;

    [SerializeField] private Buff  mCastBuff;
    [SerializeField] private float mDurate;
    [SerializeField] private uint  mLevel;

    private AttackPeriod mAttackPeriod;
    private int mAnimControlKey;

    private bool mCanTranslateDmg;

    public AbilityTable GetAbility => AbilityTable;

    public void IInit()
    {
        mAnimControlKey = Animator.GetParameter(0).nameHash;

        HealthBarPool.Instance.UsingHealthBar(-1f, transform, AbilityTable);

        mAttackPeriod = new AttackPeriod(AbilityTable);

        mAttackPeriod.SetAction(Period.Attack, CastBuff);
        mCanTranslateDmg = true;
    }

    public bool IsActive()
    {
        return gameObject.activeSelf;
    }

    public void IUpdate()
    {
        mAttackPeriod.StartPeriod();
    }

    private void CastBuff()
    {
        ICombatable[] combats = mSenseArae.GetEnterTypeT<ICombatable>();

        Anim.SetActive(true);

        for (int i = 0; i < combats.Length; ++i)
        {
            AbilityTable stat = combats[i].GetAbility;

            IEnumerator buffEnumator 
                = BuffLibrary.Instance.GetBuff(mCastBuff, mLevel, mDurate, stat);

            combats[i].CastBuff(mCastBuff, buffEnumator);
        }
        StartCoroutine(EffectAnimPlayOver());
    }

    public void PlayerEnter(MESSAGE message, Player enterPlayer) { }
    public void PlayerExit (MESSAGE message) { }

    public GameObject ThisObject() => gameObject;

    public void Damaged(float damage, GameObject attacker)
    {
        EffectLibrary.Instance.UsingEffect(EffectKind.Damage, transform.position);

        int damaged = (int)AnimState.Damaged;

        if (mCanTranslateDmg)
        {
            Animator.SetInteger(mAnimControlKey, damaged);
            mCanTranslateDmg = false;
        }

        if ((AbilityTable.Table[Ability.CurHealth] -= damage) <= 0)
        {
            Animator.SetInteger(mAnimControlKey, (int)AnimState.Death);

            HealthBarPool.Instance.UnUsingHealthBar(transform);
        }
    }

    public void CastBuff(Buff buffType, IEnumerator castedBuff)
    {
        StartCoroutine(castedBuff);
    }

    public void AnimationPlayOver(AnimState anim)
    {
        switch (anim)
        {
            case AnimState.Damaged:
                Animator.SetInteger(mAnimControlKey, (int)AnimState.Idle);
                mCanTranslateDmg = true;
                break;

            case AnimState.Death:
                gameObject.SetActive(false);
                break;
        }
    }

    private IEnumerator EffectAnimPlayOver()
    {
        for (float i = 0f; i <= AnimLength; i += Time.deltaTime * Time.timeScale)
        {
            yield return null;
        }
        mAttackPeriod.AttackActionOver();
        Anim.SetActive(false);
    }
}
