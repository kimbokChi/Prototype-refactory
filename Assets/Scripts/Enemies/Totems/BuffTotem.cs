using System.Collections;
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

    [SerializeField] private BUFF  mCastBuff;
    [SerializeField] private float mDurate;
    [SerializeField] private uint  mLevel;

    private AttackPeriod mAttackPeriod;
    private int mAnimControlKey;

    public AbilityTable GetAbility => AbilityTable;

    public void IInit()
    {
        mAnimControlKey = Animator.GetParameter(0).nameHash;

        HealthBarPool.Instance.UsingHealthBar(-1f, transform, AbilityTable);

        mAttackPeriod = new AttackPeriod(AbilityTable, AnimLength);

        mAttackPeriod.SetAction(Period.Attack, CastBuff);
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

            switch (mCastBuff)
            {
                case BUFF.HEAL:
                    combats[i].CastBuff(mCastBuff, BuffLibrary.Instance.GetBurstBUFF(BUFF.HEAL, mLevel, stat));
                    break;

                case BUFF.SPEEDUP:
                    combats[i].CastBuff(mCastBuff, BuffLibrary.Instance.GetSlowBUFF(BUFF.SPEEDUP, mLevel, mDurate, stat));
                    break;

                case BUFF.POWER_BOOST:
                    combats[i].CastBuff(mCastBuff, BuffLibrary.Instance.GetSlowBUFF(BUFF.POWER_BOOST, mLevel,mDurate, stat));
                    break;
            }
        }
        StartCoroutine(EffectAnimPlayOver());
    }

    public void PlayerEnter(MESSAGE message, Player enterPlayer) { }
    public void PlayerExit (MESSAGE message) { }

    public GameObject ThisObject() => gameObject;

    public void Damaged(float damage, GameObject attacker)
    {
        Animator.SetInteger(mAnimControlKey, (int)AnimState.Damaged);

        if ((AbilityTable.Table[Ability.CurHealth] -= damage) <= 0)
        {
            Animator.SetInteger(mAnimControlKey, (int)AnimState.Death);

            HealthBarPool.Instance.UnUsingHealthBar(transform);
        }
    }

    public void CastBuff(BUFF buffType, IEnumerator castedBuff)
    {
        StartCoroutine(castedBuff);
    }

    public void AnimationPlayOver(AnimState anim)
    {
        switch (anim)
        {
            case AnimState.Attack:
                Anim.SetActive(false);
                break;

            case AnimState.Damaged:
                Animator.SetInteger(mAnimControlKey, (int)AnimState.Idle);
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
        Anim.SetActive(false);
    }
}
