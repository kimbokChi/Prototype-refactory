using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffTotem : MonoBehaviour, IObject, ICombatable
{
    [SerializeField] private AbilityTable AbilityTable;

    [SerializeField] private Area mSenseArae;

    [SerializeField] private BUFF  mCastBuff;
    [SerializeField] private float mDurate;
    [SerializeField] private uint  mLevel;

    private AttackPeriod mAttackPeriod;

    public AbilityTable GetAbility => AbilityTable;

    public void IInit()
    {
        mAttackPeriod = new AttackPeriod(AbilityTable);

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
    }

    public void PlayerEnter(MESSAGE message, Player enterPlayer) { }
    public void PlayerExit (MESSAGE message) { }

    public GameObject ThisObject() => gameObject;

    public void Damaged(float damage, GameObject attacker)
    {
        gameObject.SetActive((AbilityTable.Table[Ability.CurHealth] -= damage) > 0f);
    }

    public void CastBuff(BUFF buffType, IEnumerator castedBuff)
    {
        StartCoroutine(castedBuff);
    }
}
