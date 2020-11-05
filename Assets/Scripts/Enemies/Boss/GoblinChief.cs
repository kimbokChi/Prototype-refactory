using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinChief : MonoBehaviour, IObject, ICombatable, IAnimEventReceiver
{
    public enum Anim
    {
        Idle, Jump, Swing, Skill
    }
    [SerializeField] private LPOSITION3 LPosition3;

    [Header("Ability")]
    [SerializeField] private AbilityTable AbilityTable;
    [SerializeField] private Animator Animator;

    [Header("Totem Skill Info")]
    [SerializeField] private SpecialTotem BuffTotem;
    [SerializeField] private SpecialTotem BombTotem;
    [SerializeField] private SpecialTotem LightningTotem;

    [Header("Swing Skill Info")]
    [SerializeField] private Area SwingArea;

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
            case AnimState.Damaged: // SummonTotem
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
            int random = Random.Range(0, 2);
            switch (random)
            {
                case 0:
                    Animator.SetInteger(mControlKey, (int)Anim.Skill);
                    break;

                case 1:
                    if (mPlayer.GetLPOSITION3() == LPosition3) {
                        DashSwing();
                    }
                    break;
            }
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

        SwingArea.SetEnterAction(o =>
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

    private void DashSwing()
    {
        var point = mPlayer.transform.position + Vector3.up * 1.05f;

        if (point.x > transform.position.x)
        {
            transform.rotation = Quaternion.Euler(Vector3.up * 180);
        }
        else
            transform.rotation = Quaternion.Euler(Vector3.zero);

        StartCoroutine(Dash(point));
    }
    private IEnumerator Dash(Vector2 point)
    {
        float lerp = 0f;

        float DeltaTime()
        {
            return Time.deltaTime * Time.timeScale;
        }
        while (lerp < 1f)
        {
            lerp = Mathf.Min(1, lerp + AbilityTable.MoveSpeed * DeltaTime());

            transform.position = Vector2.Lerp(transform.position, point, lerp);

            yield return null;
        }
        Animator.SetInteger(mControlKey, (int)Anim.Swing);
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
