using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, ICombatable
{
    [SerializeField]
    private GameObject mGameOverWindow;

    [SerializeField]
    private Animator WeaponAnimator;
    [SerializeField]
    private SpriteRenderer WeaponRenderer;

    [SerializeField]
    private float mBlinkTime;
    private Timer mBlinkTimer;

    [SerializeField] 
    private Area RangeArea;

    [SerializeField]
    private AbilityTable AbilityTable;

    [SerializeField]
    private float mDefense;

    [SerializeField]
    private DIRECTION9 mLocation9;

    private IEnumerator mEMove;

    private Inventory mInventory;

    private SpriteRenderer mRenderer;

    private List<Collider2D> mCollidersOnMove;

    private AttackPeriod mAttackPeriod;

    private event Action DeathEvent;

    private bool mCanElevation;

    private bool mIsMovingElevation;

    public bool IsDeath { get; private set; }

    public AbilityTable GetAbility => AbilityTable;

    private CircleCollider2D mRangeCollider;

    private GameObject  mTargetObject;
    private ICombatable mTargetCombat;

    private float DeltaTime
    { get => Time.deltaTime * Time.timeScale; }

    public LPOSITION3 GetLPOSITION3()
    {
        switch (mLocation9)
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

            default:
                break;
        }
        Debug.Log("Value Error");
        return LPOSITION3.NONE;
    }

    public TPOSITION3 GetTPOSITION3()
    {
        switch (mLocation9)
        {
            case DIRECTION9.TOP_LEFT:
            case DIRECTION9.MID_LEFT:
            case DIRECTION9.BOT_LEFT:
                return TPOSITION3.LEFT;

            case DIRECTION9.TOP:
            case DIRECTION9.MID:
            case DIRECTION9.BOT:
                return TPOSITION3.MID;

            case DIRECTION9.TOP_RIGHT:
            case DIRECTION9.MID_RIGHT:
            case DIRECTION9.BOT_RIGHT:
                return TPOSITION3.RIGHT;

            default:
                break;
        }
        Debug.Log("Value Error");
        return TPOSITION3.NONE;
    }


    private void Start()
    {
        mCanElevation = false;
        IsDeath       = false;

        mIsMovingElevation = false;

        mBlinkTimer = new Timer();

        mAttackPeriod = new AttackPeriod(AbilityTable);
        mAttackPeriod.SetAction(Period.Begin, () => WeaponAnimator.SetBool("PlayReverse", false));
        mAttackPeriod.SetAction(Period.Attack, AttackAction);
        mAttackPeriod.SetAction(Period.After, () => WeaponAnimator.SetBool("PlayReverse", true));

        RangeArea.SetEnterAction(SenseTarget);
        RangeArea.SetEmptyAction(() => { mTargetObject = null; mTargetCombat = null; });

        mCollidersOnMove = new List<Collider2D>();

        DeathEvent += () => mGameOverWindow.SetActive(true);
        DeathEvent += () =>      RangeArea.enabled = false;
        DeathEvent += () => WeaponAnimator.enabled = false;

        Debug.Assert(gameObject.TryGetComponent(out mRenderer));

        mInventory = Inventory.Instance;

        if (RangeArea.gameObject.TryGetComponent(out mRangeCollider))
        {
            mInventory.WeaponEquipEvent += o =>
            {               
                if (o == null)
                {
                    float PlayerData(string dataName) {
                        return float.Parse(DataUtil.GetDataValue("CharacterAbility", "ID", "Player", dataName));
                    }

                    mRangeCollider.radius = PlayerData("Range");

                    AbilityTable.Table[Ability.After_AttackDelay] = PlayerData("After_AttackDelay");
                    AbilityTable.Table[Ability.Begin_AttackDelay] = PlayerData("Begin_AttackDelay");

                    WeaponRenderer.sprite = null;
                }
                else
                {
                    float ItemData(string dataName) {
                        return float.Parse(DataUtil.GetDataValue("ItemData", "ID", o.gameObject.name, dataName));
                    }

                    mRangeCollider.radius = o.WeaponRange;

                    AbilityTable.Table[Ability.After_AttackDelay] = ItemData("Begin-AttackDelay");
                    AbilityTable.Table[Ability.Begin_AttackDelay] = ItemData("After-AttackDelay");

                    WeaponRenderer.sprite = o.Sprite;
                }
            };
        }
    }

    private void InputAction()
    {
        DIRECTION9 moveDir9 = DIRECTION9.END;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            switch (GetLPOSITION3())
            {
                case LPOSITION3.TOP:
                    mCanElevation = Castle.Instance.CanNextPoint();

                    moveDir9 = mLocation9;
                    break;

                case LPOSITION3.MID:
                case LPOSITION3.BOT:
                    {
                        mIsMovingElevation = true;

                        moveDir9 = mLocation9 - 3;
                    }
                    break;
            }
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            if (mIsMovingElevation = 
                GetLPOSITION3() != LPOSITION3.BOT) 
                moveDir9 = mLocation9 + 3;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (GetTPOSITION3() != TPOSITION3.LEFT) 
                moveDir9 = mLocation9 - 1;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            if (GetTPOSITION3() != TPOSITION3.RIGHT)
                moveDir9 = mLocation9 + 1;
        }
        if (moveDir9 != DIRECTION9.END) MoveAction(moveDir9);
    }
    private void Update()
    {
        if (!mBlinkTimer.IsOver()) 
             mBlinkTimer.Update();

        if (!IsDeath)
        {
            InputAction();
        }
        if (mTargetObject != null)
        {
            if (mTargetObject.transform.position.x > transform.position.x)
                 transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            else transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

            if (RangeArea.Has(mTargetObject)) 
            {
                mAttackPeriod.StartPeriod();
            }
            else
            {
                mTargetObject = null;
                mTargetCombat = null;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (mEMove != null) mCollidersOnMove.Add(collision);
    }

    private void SenseTarget(GameObject target)
    {
        if (target.CompareTag("Enemy")) {
            mTargetObject = mTargetObject ?? target;
        }
    }

    private void AttackAction()
    {
        if (mTargetObject != null)
        {
            if (RangeArea.Has(mTargetObject))
            {
                if (mTargetCombat == null) {
                    mTargetObject.TryGetComponent(out mTargetCombat);
                }
                mInventory.OnAttack(gameObject, mTargetCombat);
            }
            else
            {
                mTargetObject = null;
                mTargetCombat = null;
            }
        }
    }

    private void MoveAction(DIRECTION9 moveDIR9)
    {
        if (mEMove == null)
        {
            if (mCanElevation)
            {
                if (Castle.Instance.CanNextPoint(out Vector2 nextPoint))
                {
                    #region comment
                    // TOP_LEFT  -> BOT_LEFT
                    // TOP       -> BOT
                    // TOP_RIGHT -> BOT_RIGHT
                    #endregion
                    if (mLocation9 >= DIRECTION9.TOP_LEFT &&
                        mLocation9 <= DIRECTION9.TOP_RIGHT) moveDIR9 += 6;

                    StartCoroutine(mEMove = EMove(nextPoint, moveDIR9));
                }
            }
            else
            {
                if (mLocation9 - moveDIR9 < 0)
                     transform.localRotation = Quaternion.Euler(0f,   0f, 0f);
                else transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

                Vector2 movePoint = Castle.Instance.GetMovePoint(moveDIR9);

                StartCoroutine(mEMove = EMove(movePoint, moveDIR9));
            }
        }
    }

    private IEnumerator EMove(Vector2 movePoint, DIRECTION9 moveDIR9)
    {
        mInventory.OnMoveBegin(movePoint.normalized);

        for (float lerpAmount = 0f; lerpAmount < 1f; )
        {
            lerpAmount = Mathf.Min(1f, lerpAmount + DeltaTime * AbilityTable.MoveSpeed);

            transform.position = Vector2.Lerp(transform.position, movePoint, lerpAmount);

            yield return null;
        }
        mInventory.OnMoveEnd(mCollidersOnMove.ToArray());

        mCollidersOnMove.Clear();

        if (mCanElevation)
        {
            mInventory.OnFloorEnter();
            mCanElevation = false;
        }
        mIsMovingElevation = false;

        mLocation9 = moveDIR9; mEMove = null;
        yield break;
    }

    #region READ
    /// <summary>
    /// 플레이어 개체가 위/아래로 이동중이라면 false를, 그렇지 않다면 true를 반환합니다.
    /// </summary>
    /// <param name="playerPos">
    /// 함수의 반환값과는 관계없이 일관되게 플레이어의 위치를 전달합니다.
    /// </param>
    /// <returns></returns>
    #endregion
    public bool TryGetPosition(out Vector2 playerPos)
    {
        playerPos = transform.position;
            return !mIsMovingElevation;
    }

    public void Damaged(float damage, GameObject attacker)
    {
        if (mBlinkTimer.IsOver())
        {
            mBlinkTimer.Start(mBlinkTime);

            mInventory.OnDamaged(ref damage, attacker, gameObject);

                          AbilityTable.Table[Ability.CurHealth] -= damage / mDefense;
            if (IsDeath = AbilityTable.Table[Ability.CurHealth] <= 0f)
            {
                DeathEvent.Invoke();
            }
        }
    }

    public void CastBuff(BUFF buffType, IEnumerator castedBuff)
    {
        StartCoroutine(castedBuff);
    }
}
