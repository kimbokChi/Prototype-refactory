using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombTotem : MonoBehaviour, IObject, ICombatable
{
    [SerializeField] private AbilityTable AbilityTable;

    [SerializeField] private float mTriggerRadius;

    private IEnumerator mEOnFuse;

    private AttackPeriod mAttackPeriod;

    private Player mPlayer;

    public AbilityTable GetAbility => AbilityTable;

    public void IInit()
    {
        HealthBarPool.Instance.UsingHealthBar(-1f, transform, AbilityTable);

        mAttackPeriod = new AttackPeriod(AbilityTable);

        mAttackPeriod.SetAction(Period.Attack, () => StartCoroutine(mEOnFuse = EOnFuse()));
    }

    public bool IsActive()
    {
        return gameObject.activeSelf;
    }

    public void IUpdate()
    {
        if (mPlayer != null && mEOnFuse == null)
        {
            Vector2 playerPos = mPlayer.transform.position;

            if (OnTriggerPlayer())
            {
                mAttackPeriod.StartPeriod();
            }
        }
    }

    public void PlayerEnter(MESSAGE message, Player enterPlayer)
    {
        if (AbilityTable.CanRecognize(message))
            mPlayer = enterPlayer;
    }

    public void PlayerExit(MESSAGE message)
    {
        if (AbilityTable.CanRecognize(message))
            mPlayer = null;
    }

    private IEnumerator EOnFuse()
    {
        for (float i = 0; i < AbilityTable.BeginAttackDelay; i += Time.deltaTime * Time.timeScale) { yield return null; }

        if (OnTriggerPlayer())
        {
            mPlayer.Damaged(AbilityTable.AttackPower, gameObject);

        }
        mEOnFuse = null;
    }

    private bool OnTriggerPlayer()
    {
        if (mPlayer != null)
        {
            Vector2 playerPos = mPlayer.transform.position;

            return Vector2.Distance(playerPos, transform.position) <= mTriggerRadius;
        }
        return false;
    }

    public GameObject ThisObject() => gameObject;

    public void Damaged(float damage, GameObject attacker)
    {
        if ((AbilityTable.Table[Ability.CurHealth] -= damage) <= 0)
        {
            gameObject.SetActive(false);

            HealthBarPool.Instance.UnUsingHealthBar(transform);
        }
    }

    public void CastBuff(BUFF buffType, IEnumerator castedBuff)
    {
        StartCoroutine(castedBuff);
    }
}
