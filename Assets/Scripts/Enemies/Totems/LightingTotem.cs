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
            mAttackPeriod.Update();
        }
    }

    public void PlayerEnter(MESSAGE message, Player enterPlayer)
    {
        mPlayer = enterPlayer;
    }

    public void PlayerExit(MESSAGE message)
    {
        if (message.Equals(MESSAGE.BELONG_FLOOR))
        {
            mPlayer = null;
        }
    }

    public GameObject ThisObject() => gameObject;

    private void SummonLighting()
    {
        if (mPlayer.Position(out Vector2 playerPos))
        {
            mPool.Pop().SetDamage(AbilityTable.AttackPower);
        }
    }

    private void Pool_popMethod(Lighting lighting)
    {
        mPlayer.Position(out Vector2 playerPos);

        lighting.transform.position = mLightingOffset + playerPos;

        lighting.gameObject.SetActive(true);
    }

    public void Damaged(float damage, GameObject attacker)
    {
        gameObject.SetActive((AbilityTable.Table[Ability.CurHealth] -= damage) > 0f);
    }

    public void CastBuff(BUFF buffType, IEnumerator castedBuff)
    {
        StartCoroutine(castedBuff);
    }
}
