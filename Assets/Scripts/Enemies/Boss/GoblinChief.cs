using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GoblinChief : MonoBehaviour, IObject, ICombatable
{
    private const int LIGHTNING_CNT = 3;

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
    [SerializeField] private SBombTotem BombTotem;
    [SerializeField] private SLightningTotemSkill LightningTotemSkill;

    private Queue<SBombTotem> mBombTotems;
    private Queue<SLightningTotemSkill> mLightningSkills;

    [Header("Swing Skill Info")]
    [SerializeField] private Collider2D DashCollider;
    [SerializeField] private Area SwingArea;

    [Header("Summon Goblins")]
    [SerializeField] private GameObject[] Goblins;

    private Player mPlayer;
    private int mControlKey;

    private Anim mNextPattern;
    private DIRECTION9 mJumpDIR9;
    private AttackPeriod mAttackPeriod;

    public AbilityTable GetAbility => AbilityTable;

    private void ChangeIdleState()
    {
        Animator.SetInteger(mControlKey, (int)Anim.Idle);
    }
    private void PatternActionOver()
    {
        mAttackPeriod.AttackActionOver();

        Animator.SetInteger(mControlKey, (int)Anim.Idle);
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

        BombTotem.transform.parent = null;
        BuffTotem.transform.parent = null;

        BombTotem.Init();

        mNextPattern = (Anim)Random.Range(2, 4);

        mAttackPeriod = new AttackPeriod(AbilityTable);
        mAttackPeriod.SetAction(Period.Attack, () =>
        {
            switch (mNextPattern)
            {
                case Anim.Skill:
                    Animator.SetInteger(mControlKey, (int)Anim.Skill);
                    break;

                case Anim.Swing:
                    if (mPlayer.GetLPOSITION3() == LPosition3)
                    {
                        DashSwing();
                    }
                    else
                    {
                        Animator.SetInteger(mControlKey, (int)Anim.Jump);
                    }
                    break;
            }
            mNextPattern = (Anim)Random.Range(2, 4);
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

        #region Totem Skill Init
        mLightningSkills 
            = new Queue<SLightningTotemSkill>();

        for (int i = 0; i < 2; i++)
            AddLightningSkill();

        mBombTotems 
            = new Queue<SBombTotem>();

        for (int i = 0; i < 2; i++)
            AddBombTotem();
        #endregion

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
        if (!mAttackPeriod.IsProgressing()) 
        {
            mAttackPeriod.StartPeriod();
        }
    }

    private void Jumping()
    {
        mJumpDIR9 = mPlayer.GetDIRECTION9();

        LPOSITION3 Dir2LPos(DIRECTION9 dir)
        {
            switch (dir)
            {
                case DIRECTION9.TOP_LEFT:
                case DIRECTION9.TOP:
                case DIRECTION9.TOP_RIGHT:
                    return LPOSITION3.TOP;

                case DIRECTION9.MID_LEFT:
                case DIRECTION9.MID:
                case DIRECTION9.MID_RIGHT:
                    return LPOSITION3.MID;

                case DIRECTION9.BOT_LEFT:
                case DIRECTION9.BOT:
                case DIRECTION9.BOT_RIGHT:
                    return LPOSITION3.BOT;
            }
            return LPOSITION3.NONE;
        }

        float moveY = Castle.Instance.GetMovePoint(mJumpDIR9).y;

        Vector2 point = new Vector2(transform.position.x, moveY + 1.05f);

        if (point.x > transform.position.x)
        {
            transform.rotation = Quaternion.Euler(Vector3.up * 180);
        }
        else
            transform.rotation = Quaternion.Euler(Vector3.zero);

        LPosition3 = Dir2LPos(mJumpDIR9);

        StartCoroutine(Move(point, () =>
        {
            Animator.SetInteger(mControlKey, (int)Anim.Landing);
        }));
    }

    private void DashSwing()
    {
        float moveX = Castle.Instance.GetMovePoint(mPlayer.GetDIRECTION9()).x;

        Vector2 point = new Vector2(moveX, transform.position.y);

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
    private IEnumerator SummonBuff(Vector2 castPoint)
    {
        yield return new WaitForSeconds(BuffTotem.EffectPlayTime);

        Room room = Castle.Instance.GetPlayerRoom();

        for (int i = -1; i < 2; i++)
        {
            var goblin = Instantiate(Goblins[Random.Range(0, Goblins.Length)], room.transform);

            if (goblin.TryGetComponent(out IObject iobject))
            {
                room.AddIObject(iobject);
            }
            goblin.transform.localPosition += Vector3.right * castPoint.x;
            goblin.transform.localPosition += Vector3.left * i;
        }
        yield return new WaitForSeconds(0.583f);
        mAttackPeriod.AttackActionOver();
    }

    private void SummonTotem()
    {
        int random = Random.Range(0, 3);

        DIRECTION9 playerDIR9 = mPlayer.GetDIRECTION9();

        Vector2 castPoint = new Vector2
            (mPlayer.transform.position.x, Castle.Instance.GetMovePoint(playerDIR9).y + 1.1f);

        SpecialTotem totem = null;

        switch (random) {
            case 0:
                if (mBombTotems.Count == 0) 
                {
                    AddBombTotem();
                }
                mBombTotems.Dequeue().Cast(castPoint);
                break;

            case 1:
                totem = BuffTotem;
                {
                    Room room = Castle.Instance.GetPlayerRoom();

                    for (int i = -1; i < 2; i++)
                    {
                        var goblin = Instantiate(Goblins[Random.Range(0, Goblins.Length)], room.transform);

                        if (goblin.TryGetComponent(out IObject iobject)) 
                        {
                            room.AddIObject(iobject);
                        }
                        goblin.transform.localPosition += Vector3.right * castPoint.x;
                        goblin.transform.localPosition += Vector3.left  * i;
                    }
                    mAttackPeriod.AttackActionOver();
                }
                break;

            case 2:
                if (mLightningSkills.Count == 0)
                {
                    AddLightningSkill();
                }
                mLightningSkills.Dequeue().Cast();
                break;
        }
        totem?.CastSkill(castPoint);
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
    private void AddBombTotem()
    {
        var bombTotem = Instantiate(BombTotem);

        bombTotem.Init();
        bombTotem.CastOverAction = o =>
        { 
            mBombTotems.Enqueue(o);
        };
        mBombTotems.Enqueue(bombTotem);
    }
    private void AddLightningSkill()
    {
        var lightning = Instantiate(LightningTotemSkill);

        lightning.Init();
        lightning.CastOverAction = o =>
        {
            mLightningSkills.Enqueue(o);
        };
        mLightningSkills.Enqueue(lightning);
    }
}
