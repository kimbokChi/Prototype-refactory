using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingTotem : MonoBehaviour, IObject, ICombatable
{
    [SerializeField] private AbilityTable AbilityTable;

    [SerializeField] private Lighting mLighting;

    [SerializeField] private Vector2 mLightingOffset;

    private Player mPlayer;

    private AttackPeriod mAttackPeriod;

    private Pool<Lighting> mPool;

    public AbilityTable GetAbility => AbilityTable;

    public void IInit()
    {
        HealthBarPool.Instance.UsingHealthBar(-1f, transform, AbilityTable);

        mAttackPeriod = new AttackPeriod(AbilityTable);
        mAttackPeriod.SetAction(Period.Attack, SummonLighting);

        mPool = new Pool<Lighting>();
        mPool.Init(mLighting, Pool_popMethod, null, o => o.CanDisable());
    }

    public bool IsActive()
    {
        return gameObject.activeSelf;
    }

    public void IUpdate()
    {
        mPool.Update();

        if (mPlayer != null)
        {
            mAttackPeriod.StartPeriod();
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

    public GameObject ThisObject() => gameObject;

    private void SummonLighting()
    {
        if (mPlayer.TryGetPosition(out Vector2 playerPos))
        {
            mPool.Pop().SetDamage(AbilityTable.AttackPower);
        }
    }

    private void Pool_popMethod(Lighting lighting)
    {
        mPlayer.TryGetPosition(out Vector2 playerPos);

        lighting.transform.position = mLightingOffset + playerPos;

        lighting.gameObject.SetActive(true);
    }

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
