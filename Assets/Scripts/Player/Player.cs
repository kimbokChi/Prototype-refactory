﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class InputExtension
{
    // 현재 입력이 'UI를 대상으로한 입력인가'의 여부를 반환
    public static bool IsPointerInUIObject(this EventSystem eventSystem)
    {
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.WindowsEditor:
                return eventSystem.IsPointerOverGameObject();

            case RuntimePlatform.Android:
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);

                    return eventSystem.IsPointerOverGameObject(touch.fingerId);
                }
                return false;
        }
        return false;
    }
}

public class Player : MonoBehaviour, ICombatable
{
    public event Action<LPOSITION3, float> MovingEvent;

    [SerializeField] private bool CanMoveDown;
    [SerializeField] private bool IsUsingHealthBar;
    
    [SerializeField]
    private GameObject mGameOverWindow;

    [SerializeField]
    private GameObject EquipWeaponSlot;

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
        if (IsUsingHealthBar)
        {
            HealthBarPool.Instance?.UsingPlayerHealthBar(-1f, transform, AbilityTable);
        }
        mIsInputLock  = false;
        mCanElevation = false;
        IsDeath       = false;

        mIsMovingElevation = false;

        mAttackPeriod = new AttackPeriod(AbilityTable);
        mAttackPeriod.SetAction(Period.Attack, AttackAction);

        mCollidersOnMove = new List<Collider2D>();

        DeathEvent += () => mGameOverWindow.SetActive(true);
        DeathEvent += () =>      RangeArea.enabled = false;
        DeathEvent += () => HealthBarPool.Instance?.UnUsingHealthBar(transform);
        DeathEvent += () => PlayerAnimator.ChangeState(PlayerAnim.Death);
        DeathEvent += () => mInventory.Clear();
        DeathEvent += () =>
        {
            if (EquipWeaponSlot.transform.childCount != 0)
            {
                EquipWeaponSlot.transform.GetChild(0).transform.parent = ItemStateSaver.Instance.transform;
            }
            ItemLibrary.Instance.ItemBoxReset();
        };

        Debug.Assert(gameObject.TryGetComponent(out mRenderer));

        mInventory = Inventory.Instance;

        if (RangeArea.gameObject.TryGetComponent(out mRangeCollider))
        {
            mInventory.WeaponEquipEvent += o =>
            {               
                if (o == null)
                {
                    mRangeCollider.radius = AbilityTable.GetAblity[Ability.Range];

                    AbilityTable.Table[Ability.After_AttackDelay] = AbilityTable.GetAblity[Ability.After_AttackDelay];
                    AbilityTable.Table[Ability.Begin_AttackDelay] = AbilityTable.GetAblity[Ability.Begin_AttackDelay];

                    mAttackPeriod.StopPeriod();
                }
                else
                {
                    o.transform.parent = EquipWeaponSlot.transform;

                    o.transform.localRotation = Quaternion.Euler(Vector3.zero);
                    o.transform.localScale    = Vector3.one;
                    o.transform.localPosition = Vector3.zero;

                    mRangeCollider.radius = o.WeaponRange;

                    AbilityTable.Table[Ability.After_AttackDelay] = o.After_AttackDelay;
                    AbilityTable.Table[Ability.Begin_AttackDelay] = o.Begin_AttackDelay;

                    o.AttackOverAction = () => mAttackPeriod.AttackActionOver();

                    ItemStateSaver.Instance.SaveSlotItem(SlotType.Weapon, o, 0);
                    mAttackPeriod.StopPeriod();
                }
            };
            mInventory.WeaponChangeEvent += o =>
            {
                o.transform.parent   = ItemStateSaver.Instance.transform;
                o.transform.position = new Vector3(-10, 0, 0);

                mAttackPeriod.StopPeriod();
            };
        }
        mInventory.SetWeaponSlot(ItemStateSaver.Instance.LoadSlotItem(SlotType.Weapon, 0));
    }

    private void InputAction()
    {
        if (!mIsInputLock)
        {
            DIRECTION9 moveDir9 = DIRECTION9.END;

            if (Input.GetKey(KeyCode.UpArrow) || Finger.Instance.Swipe(SwipeDirection.up))
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
            else if (Input.GetKey(KeyCode.DownArrow) || Finger.Instance.Swipe(SwipeDirection.down))
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
                            if (CanMoveDown)
                            {
                                mCanElevation = Castle.Instance.CanPrevPoint();

                                moveDir9 = mLocation9;
                            }
                        }
                        break;
                }
            }
            else if (Input.GetKey(KeyCode.LeftArrow) || Finger.Instance.Swipe(SwipeDirection.left))
            {
                if (GetTPOSITION3() != TPOSITION3.LEFT)
                    moveDir9 = mLocation9 - 1;
            }
            else if (Input.GetKey(KeyCode.RightArrow) || Finger.Instance.Swipe(SwipeDirection.right))
            {
                if (GetTPOSITION3() != TPOSITION3.RIGHT)
                    moveDir9 = mLocation9 + 1;
            }
            if (moveDir9 != DIRECTION9.END) MoveAction(moveDir9);
        }
    }
    private void Update()
    {
        if (!IsDeath)
        {
            InputAction();
        }
        if (mEMove == null)
        {
            Vector2 interactionPoint = Vector2.zero;

            if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
            {
                if (!EventSystem.current.IsPointerInUIObject())
                {
                    switch (Application.platform)
                    {
                        case RuntimePlatform.WindowsPlayer:
                        case RuntimePlatform.WindowsEditor:
                            if (Input.GetMouseButtonDown(0))
                            {
                                interactionPoint =
                                    Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            }
                            break;

                        case RuntimePlatform.Android:
                            if (Input.touchCount > 0)
                            {
                                interactionPoint =
                                    Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                            }
                            break;
                    }
                    SetLookAtLeft(interactionPoint.x < 0);
                }

            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (mEMove != null) mCollidersOnMove.Add(collision);
    }

    public void SetLookAtLeft(bool lookLeft)
    {
        if (!mAttackPeriod.IsProgressing())
        {
            if (lookLeft)
            {
                transform.localRotation = Quaternion.Euler(Vector3.up * 180f);
            }
            else
                transform.localRotation = Quaternion.Euler(Vector3.zero);
        }
    }

    public void AttackOrder()
    {
        if (mInventory.IsEquipWeapon())
        {
            if (!mAttackPeriod.IsProgressing())
            {

                mAttackPeriod.StartPeriod();
            }
        }
        
    }

    private void AttackAction()
    {
        mInventory.AttackAction(gameObject, null);
    }

    private void MoveAction(DIRECTION9 moveDIR9)
    {
        if (mEMove == null && mAttackPeriod.CurrentPeriod == Period.Begin)
        {
            mAttackPeriod.StopPeriod();

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
                         bool lookAtLeft = mLocation9 - moveDIR9 > 0;
                SetLookAtLeft(lookAtLeft);

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

            LPOSITION3 moveDIR3 = LPOSITION3.NONE;

            switch (moveDIR9)
            {
                case DIRECTION9.TOP_LEFT:
                case DIRECTION9.TOP:
                case DIRECTION9.TOP_RIGHT:
                    moveDIR3 = LPOSITION3.TOP;
                    break;
                case DIRECTION9.MID_LEFT:
                case DIRECTION9.MID:
                case DIRECTION9.MID_RIGHT:
                    moveDIR3 = LPOSITION3.MID;
                    break;
                case DIRECTION9.BOT_LEFT:
                case DIRECTION9.BOT:
                case DIRECTION9.BOT_RIGHT:
                    moveDIR3 = LPOSITION3.BOT;
                    break;
            }
            MovingEvent?.Invoke(moveDIR3, lerpAmount);

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
        MainCamera.Instance.Shake(0.3f, 1.0f);

        mInventory.OnDamaged(ref damage, attacker, gameObject);

        AbilityTable.Table[Ability.CurHealth] -= damage / mDefense;
        if (IsDeath = AbilityTable.Table[Ability.CurHealth] <= 0f)
        {
            DeathEvent?.Invoke();
            DeathEvent = null;
        }
        if (damage > 0f)
        {
            EffectLibrary.Instance.UsingEffect(EffectKind.Damage, transform.position);
        }
    }

    public void CastBuff(Buff buffType, IEnumerator castedBuff)
    {
        StartCoroutine(castedBuff);
    }
}
