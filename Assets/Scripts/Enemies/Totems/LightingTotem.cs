using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingTotem : MonoBehaviour, IObject, ICombatable, IAnimEventReceiver
{
    [SerializeField] private AbilityTable AbilityTable;

    [SerializeField] private EnemyAnimator EnemyAnimator;

    [SerializeField] private Lighting mLighting;

    [SerializeField] private Vector2 mLightingOffset;

    private Player mPlayer;

    private AttackPeriod mAttackPeriod;

    private Pool<Lighting> mPool;

    public AbilityTable GetAbility => AbilityTable;

    public void IInit()
    {
        EnemyAnimator.Init();
        HealthBarPool.Instance.UsingHealthBar(-1f, transform, AbilityTable);

        mAttackPeriod = new AttackPeriod(AbilityTable, 0.5f);

        mAttackPeriod.SetAction(Period.Attack, () => {
            EnemyAnimator.ChangeState(AnimState.Attack);
        });

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

        if (mPlayer != null) {
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

    private void AttackAction()
    {
        MainCamera.Instance.Shake(0.2f, 0.6f, true);

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
        EnemyAnimator.ChangeState(AnimState.Damaged);

        if ((AbilityTable.Table[Ability.CurHealth] -= damage) <= 0)
        {
            EnemyAnimator.ChangeState(AnimState.Death);

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
            case AnimState.Damaged:
                EnemyAnimator.ChangeState(AnimState.Idle);
                break;
            case AnimState.Death:
                gameObject.SetActive(false);
                break;
        }
    }
}
