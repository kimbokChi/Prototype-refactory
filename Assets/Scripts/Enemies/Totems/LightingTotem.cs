using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingTotem : MonoBehaviour, IObject, ICombatable
{
    [SerializeField] private AbilityTable AbilityTable;

    [SerializeField] private Lighting mLighting;

    [SerializeField] private Vector2 mLightingOffset;

    private Player mPlayer;

    private Timer mWaitForLighting;

    private Pool<Lighting> mPool;

    public AbilityTable GetAbility => AbilityTable;

    public void IInit()
    {
        mWaitForLighting = new Timer();

        mWaitForLighting.Start(AbilityTable.BeginAttackDelay);

        mPool = new Pool<Lighting>();
        mPool.Init(mLighting, Pool_popMethod, Pool_addMethod, Pool_returnToPool);
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
            if (mWaitForLighting.IsOver())
            {
                if (mPlayer.Position(out Vector2 playerPos))
                {
                    mPool.Pop().SetDamage(AbilityTable.AttackPower);

                    mWaitForLighting.Start(AbilityTable.AfterAttackDelay);
                }
            }
            else
            {
                mWaitForLighting.Update();
            }
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

    private void Pool_popMethod(Lighting lighting)
    {
        mPlayer.Position(out Vector2 playerPos);

        lighting.transform.position = mLightingOffset + playerPos;

        lighting.gameObject.SetActive(true);
    }
    private void Pool_addMethod(Lighting lighting)
    {
        lighting.gameObject.SetActive(false);
    }
    private bool Pool_returnToPool(Lighting lighting)
    {
        lighting.DurateCheck();

        return !lighting.gameObject.activeSelf;
    }

    public void Damaged(float damage, GameObject attacker)
    {
        if ((AbilityTable.Table[Ability.CurHealth] -= damage) <= 0f)
        {
            gameObject.SetActive(false);
        }
    }

    public void CastBuff(BUFF buffType, IEnumerator castedBuff)
    {
        StartCoroutine(castedBuff);
    }
}
