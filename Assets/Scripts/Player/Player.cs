using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, ICombatable
{
    [SerializeField]
    private bool CanMoveDown;
    
    [SerializeField]
    private GameObject mGameOverWindow;

    [SerializeField]
    private GameObject EquipWeaponSlot;

    [SerializeField]
    private float mBlinkTime;
    private Timer mBlinkTimer;

    [SerializeField] 
    private Area RangeArea;

    [SerializeField]
    private AbilityTable AbilityTable;

    [SerializeField]
    private PlayerAnimator PlayerAnimator;

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

    private bool mIsInputLock;

    public bool IsDeath { get; private set; }

    public AbilityTable GetAbility => AbilityTable;

    private CircleCollider2D mRangeCollider;

    private float DeltaTime
    { get => Time.deltaTime * Time.timeScale; }

    public DIRECTION9 GetDIRECTION9()
    {
        return mLocation9;
    }

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
        PlayerAnimator.Init();

        HealthBarPool.Instance?.UsingHealthBar(-0.8f, transform, AbilityTable);

        mIsInputLock  = false;
        mCanElevation = false;
        IsDeath       = false;

        mIsMovingElevation = false;

        mBlinkTimer = new Timer();

        mAttackPeriod = new AttackPeriod(AbilityTable);
        mAttackPeriod.SetAction(Period.Attack, AttackAction);

        mCollidersOnMove = new List<Collider2D>();

        DeathEvent += () => mGameOverWindow.SetActive(true);
        DeathEvent += () =>      RangeArea.enabled = false;
        DeathEvent += () => HealthBarPool.Instance?.UnUsingHealthBar(transform);
        DeathEvent += () => PlayerAnimator.ChangeState(PlayerAnim.Death);

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

                    mAttackPeriod.StopPeriod();
                }
                else
                {
                    o.transform.parent = EquipWeaponSlot.transform;

                    o.transform.localRotation = Quaternion.Euler(Vector3.zero);
                    o.transform.localScale    = Vector3.one;
                    o.transform.localPosition = Vector3.zero;

                    float ItemData(string dataName) {
                        return float.Parse(DataUtil.GetDataValue("ItemData", "ID", o.GetType().ToString(), dataName));
                    }

                    mRangeCollider.radius = o.WeaponRange;

                    AbilityTable.Table[Ability.After_AttackDelay] = ItemData("Begin-AttackDelay");
                    AbilityTable.Table[Ability.Begin_AttackDelay] = ItemData("After-AttackDelay");

                    o.AttackOverAction = () => mAttackPeriod.AttackActionOver();
                }
            };
        }
    }

    private void InputAction()
    {
        if (!mIsInputLock)
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
                switch (GetLPOSITION3())
                {
                    case LPOSITION3.TOP:
                    case LPOSITION3.MID:
                        {
                            mIsMovingElevation = true;

                            moveDir9 = mLocation9 + 3;
                        }
                        break;
                    case LPOSITION3.BOT:
                        {
                            mCanElevation = Castle.Instance.CanPrevPoint();

                            moveDir9 = mLocation9;
                        }
                        break;
                }
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
    }
    private void Update()
    {
        if (!mBlinkTimer.IsOver()) 
             mBlinkTimer.Update();

        if (!IsDeath)
        {
            InputAction();
        }
        if (RangeArea.CloestTargetPos().x > transform.position.x)
             transform.localRotation = Quaternion.Euler(Vector3.zero);
        else transform.localRotation = Quaternion.Euler(Vector3.up * 180f);

        if (mInventory.IsEquipWeapon())
        {
            if (RangeArea.HasAny())
            {
                mAttackPeriod.StartPeriod();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (mEMove != null) mCollidersOnMove.Add(collision);
    }

    private void AttackAction()
    {
        mInventory.AttackAction(gameObject, null);
    }

    private void MoveAction(DIRECTION9 moveDIR9)
    {
        if (mEMove == null)
        {
            if (mCanElevation)
            {
                switch (moveDIR9)
                {
                    case DIRECTION9.TOP_LEFT:
                    case DIRECTION9.TOP:
                    case DIRECTION9.TOP_RIGHT:
                        if (Castle.Instance.CanNextPoint(out Vector2 nextPoint)) {
                            StartCoroutine(mEMove = EMove(nextPoint, moveDIR9 + 6));
                        }
                        break;

                    case DIRECTION9.BOT_LEFT:
                    case DIRECTION9.BOT:
                    case DIRECTION9.BOT_RIGHT:
                        if (Castle.Instance.CanPrevPoint(out Vector2 prevPoint)) {
                            StartCoroutine(mEMove = EMove(prevPoint, moveDIR9 - 6));
                        }
                        break;
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
        bool a = false;

        if (mIsMovingElevation)
        {
            PlayerAnimator.ChangeState(PlayerAnim.Jump);

            a = true;
        }
        else 
            PlayerAnimator.ChangeState(PlayerAnim.Move);

        mInventory.OnMoveBegin(movePoint.normalized);

        for (float lerpAmount = 0f; lerpAmount < 1f; )
        {
            lerpAmount = Mathf.Min(1f, lerpAmount + DeltaTime * AbilityTable.MoveSpeed);

            transform.position = Vector2.Lerp(transform.position, movePoint, lerpAmount);

            if (a) {
                if (mIsMovingElevation && lerpAmount >= 0.1f)
                {
                    a = false;
                    PlayerAnimator.ChangeState(PlayerAnim.Landing);
                }
            }
            yield return null;
        }
        PlayerAnimator.ChangeState(PlayerAnim.Idle);

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

    public void InputLock(bool isLock) {
        mIsInputLock = isLock;
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

            MainCamera.Instance.Shake(0.16f, 0.5f, true);

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
