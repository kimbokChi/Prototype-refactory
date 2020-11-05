using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GoblinChief : MonoBehaviour, IObject, ICombatable, IAnimEventReceiver
{
    public enum Anim
    {
        Idle, Jump, Swing, Skill, Landing, End
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
    [SerializeField] private Collider2D DashCollider;
    [SerializeField] private Area SwingArea;

    [Header("Summon Goblins")]
    [SerializeField] private GameObject[] Goblins;

    private Player mPlayer;
    private int mControlKey;

    private Anim mNextPattern;

    private AttackPeriod mAttackPeriod;

    public AbilityTable GetAbility => AbilityTable;

    public void AnimationPlayOver(AnimState anim)
    {
        switch (anim)
        {
            case AnimState.Move: // Jump
            case AnimState.Attack:  // Swing
            case AnimState.Damaged: // SummonTotem
                Animator.SetInteger(mControlKey, (int)Anim.Idle);
                break;
        }
    }

    public void Damaged(float damage, GameObject attacker)
    {
        EffectLibrary.Instance.UsingEffect(EffectKind.EnemyDmgEffect, transform.position);

        if ((AbilityTable.Table[Ability.CurHealth] -= damage) <= 0)
        {
            gameObject.SetActive(false);

            HealthBarPool.Instance.UnUsingHealthBar(transform);
        }
    }

    public void IInit()
    {
        HealthBarPool.Instance.UsingHealthBar(-2.2f, transform, AbilityTable);

        mNextPattern = (Anim)Random.Range(1, (int)Anim.End);

        mAttackPeriod = new AttackPeriod(AbilityTable, 1.5f);
        mAttackPeriod.SetAction(Period.Attack, () =>
        {
            switch (mNextPattern)
            {
                case Anim.Skill:
                    mAttackPeriod.SetAttackTime(1.083f);
                    Animator.SetInteger(mControlKey, (int)Anim.Skill);
                    break;

                case Anim.Jump:
                case Anim.Swing:
                    if (mPlayer.GetLPOSITION3() == LPosition3) 
                    {
                        DashSwing();
                        mAttackPeriod.SetAttackTime(1.4f);
                    }
                    else
                    {
                        mAttackPeriod.SetAttackTime(2.4f);
                        Animator.SetInteger(mControlKey, (int)Anim.Jump);
                    }
                    break;
            }
            mNextPattern = (Anim)Random.Range(0, (int)Anim.End);
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

    private void Jumping()
    {
        var point = mPlayer.transform.position + Vector3.up * 1.05f;
            point.x = transform.localPosition.x;

        if (point.x > transform.position.x)
        {
            transform.rotation = Quaternion.Euler(Vector3.up * 180);
        }
        else
            transform.rotation = Quaternion.Euler(Vector3.zero);

        LPosition3 = mPlayer.GetLPOSITION3();

        StartCoroutine(Move(point, () =>
        {
            Animator.SetInteger(mControlKey, (int)Anim.Landing);
        }));
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

        StartCoroutine(Move(point, () =>
        {
            Animator.SetInteger(mControlKey, (int)Anim.Swing);

        }));
    }
    private IEnumerator Move(Vector2 point, System.Action moveOverAction)
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
        moveOverAction.Invoke();
    }

    private void SummonTotem()
    {
        int random = Random.Range(0, 3);

        Vector2 castPoint = mPlayer.transform.position + Vector3.up * 1.1f;

        SpecialTotem totem = null;

        switch (random) {
            case 0:
                totem = BombTotem;
                break;

            case 1:
                totem = BuffTotem;
                {
                    Room room = Castle.Instance.GetPlayerRoom();

                    for (int i = 0; i < 3; i++)
                    {
                        var goblin = Instantiate(Goblins[Random.Range(0, Goblins.Length)], room.transform);

                        if (goblin.TryGetComponent(out IObject iobject)) 
                        {
                            room.AddIObject(iobject);
                        }
                    }
                }
                break;

            case 2:
                totem = LightningTotem;              
                break;
        }
        totem.CastSkill(castPoint);
        mAttackPeriod.SetAttackTime(totem.PlayTime - 0.5f);
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collision.TryGetComponent(out ICombatable combatable))
            {
                combatable.Damaged(8f, gameObject);

                MainCamera.Instance.Shake(0.5f, 0.5f, true);
            }
        }
    }
}
