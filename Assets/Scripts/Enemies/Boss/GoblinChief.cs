using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinChief : MonoBehaviour, IObject, ICombatable, IAnimEventReceiver
{
    public enum Anim
    {
        Idle, Jump, Swing, Skill
    }
    [Header("Ability")]
    [SerializeField] private AbilityTable AbilityTable;
    [SerializeField] private Animator Animator;

    [Header("Skill Info")]
    [SerializeField] private SpecialTotem BuffTotem;
    [SerializeField] private SpecialTotem BombTotem;
    [SerializeField] private SpecialTotem LightningTotem;

    private Player mPlayer;
    private int mControlKey;

    private AttackPeriod mAttackPeriod;

    public AbilityTable GetAbility => AbilityTable;

    public void AnimationPlayOver(AnimState anim)
    {
        switch (anim)
        {
            case AnimState.Move: // Jump
                break;

            case AnimState.Attack:  // Swing
            case AnimState.Damaged: // Skill
                Animator.SetInteger(mControlKey, (int)Anim.Idle);
                break;
        }
    }

    public void Damaged(float damage, GameObject attacker)
    {
        
    }

    public void IInit()
    {
        mAttackPeriod = new AttackPeriod(AbilityTable, 1.5f);
        mAttackPeriod.SetAction(Period.Attack, () =>
        {
            SummonTotem();
        });
        BuffTotem.SetAreaEnterAction(o =>
        {
            if (o.TryGetComponent(out ICombatable combatable))
            {
                var ability = combatable.GetAbility;
                var buffLib = BuffLibrary.Instance;

                combatable.CastBuff(BUFF.POWER_BOOST, 
                    buffLib.GetSlowBUFF(BUFF.POWER_BOOST, 3, 5f, ability));

                combatable.CastBuff(BUFF.SPEEDUP, 
                    buffLib.GetSlowBUFF(BUFF.SPEEDUP, 3, 5f, ability));
            }
        });

        BombTotem.SetAreaEnterAction(o =>
        {
            if (o.TryGetComponent(out ICombatable combatable))
            {
                combatable.Damaged(20f, gameObject);
            }
        });

        mControlKey = Animator.GetParameter(0).nameHash;
    }

    public void IUpdate()
    {
        mAttackPeriod.StartPeriod();
    }

    private void SummonTotem()
    {
        int random = Random.Range(0, 3);

        Vector2 castPoint = mPlayer.transform.position + Vector3.up * 1.1f;

        switch (random)
        {
            case 0:
                BombTotem.CastSkill(castPoint);
                break;
            case 1:
                BuffTotem.CastSkill(castPoint);
                break;
            case 2:
                LightningTotem.CastSkill(castPoint);
                break;
        }
    }

    public void PlayerEnter(MESSAGE message, Player enterPlayer)
    {
        mPlayer = enterPlayer;
    }

    public void PlayerExit(MESSAGE message)
    {
        if (message.Equals(MESSAGE.BELONG_FLOOR)) {
            mPlayer = null;
        }
    }

    public void CastBuff(BUFF buffType, IEnumerator castedBuff) {
        StartCoroutine(castedBuff);
    }

    public bool IsActive() {
        return gameObject.activeSelf;
    }

    public GameObject ThisObject() {
        return gameObject;
    }
}
