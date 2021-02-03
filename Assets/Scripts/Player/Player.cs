using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using BackEnd;
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
    #region COMMENT
    /// <summary>
    /// parameter1 : revert death event?
    /// </summary>
    #endregion
    public event Action<bool> DeathEvent;
    public event Action<LPOSITION3, float> MovingEvent;

    [SerializeField] private bool CanMoveDown;
    [SerializeField] private bool IsUsingHealthBar;
    
    [SerializeField]
    private Resurrectable _Resurrectable;

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

    private bool mCanElevation;

    private bool mIsMovingElevation;

    private bool mIsInputLock;

    private Coroutine _MoveRoutine;

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

        mIsMovingElevation = false;

        mAttackPeriod = new AttackPeriod(AbilityTable);
        mAttackPeriod.SetAction(Period.Attack, AttackAction);

        mCollidersOnMove = new List<Collider2D>();
        Debug.Assert(gameObject.TryGetComponent(out mRenderer));

        mInventory = Inventory.Instance;

        _MoveRoutine = _MoveRoutine ?? new Coroutine(this);

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
        var instance = ItemStateSaver.Instance.LoadSlotItem(SlotType.Weapon, 0);
        if (instance != null) {
            instance = ItemLibrary.Instance.GetItemObject(instance.ID);
        } mInventory.SetWeaponSlot(instance);

        _Resurrectable.ResurrectAction += ResurrectAction;
    }
    private void InputAction()
    {
        if (!mIsInputLock)
        {
            DIRECTION9 moveDir9 = DIRECTION9.END;

            if (Input.GetKey(KeyCode.UpArrow) || Finger.Instance.Swipe(Direction.Up))
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
            else if (Input.GetKey(KeyCode.DownArrow) || Finger.Instance.Swipe(Direction.Down))
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
            else if (Input.GetKey(KeyCode.LeftArrow) || Finger.Instance.Swipe(Direction.Left))
            {
                if (GetTPOSITION3() != TPOSITION3.LEFT)
                    moveDir9 = mLocation9 - 1;
            }
            else if (Input.GetKey(KeyCode.RightArrow) || Finger.Instance.Swipe(Direction.Right))
            {
                if (GetTPOSITION3() != TPOSITION3.RIGHT)
                    moveDir9 = mLocation9 + 1;
            }
            if (moveDir9 != DIRECTION9.END) MoveAction(moveDir9);
        }
    }
    private void OnDestroy()
    {
        // 마을에서 다른 씬으로 이동하는 것이 아니라면, 인벤토리를 비운다.
        if (SceneManager.GetActiveScene().buildIndex != (int)SceneIndex.Town)
        {
            var list = new List<int>()
                {
                    // ___Weapon___
                    (int)ItemID.None,
                    
                    // ___Accessory___
                    (int)ItemID.None, (int)ItemID.None, (int)ItemID.None,

                    // ___Container___
                    (int)ItemID.None, (int)ItemID.None, (int)ItemID.None,
                    (int)ItemID.None, (int)ItemID.None, (int)ItemID.None
                };
            ItemStateSaver.Instance.SetInventoryItem(list);
        }
    }
    private void Update()
    {
        if (AbilityTable[Ability.CurHealth] > 0f)
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
    private void ResurrectAction()
    {
        RangeArea.enabled = true;
        PlayerAnimator.ChangeState(PlayerAnim.Idle);

        if (TryGetComponent(out Collider2D collider))
        {
            collider.enabled = true;
        }
        DeathEvent?.Invoke(true);

        AbilityTable.Table[Ability.CurHealth] 
            = AbilityTable[Ability.MaxHealth] * 0.3f;
    }
    private void DeathAction()
    {
        RangeArea.enabled = false;
        PlayerAnimator.ChangeState(PlayerAnim.Death);

        if (TryGetComponent(out Collider2D collider)) {

            collider.enabled = false;
        }
        DeathEvent?.Invoke(false);
    }

    public void MoveStop()
    {
        _MoveRoutine.StopRoutine();
    }

    public void MoveOrder(Direction direction)
    {
        if (!mIsInputLock)
        {
            Vector2 movePoint = Vector2.zero;

            DIRECTION9 moveDir9 = DIRECTION9.END;
            switch (direction)
            {
                case Direction.Up:
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
                    break;
                case Direction.Down:
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
                    break;
                case Direction.Right:
                    _MoveRoutine.StartRoutine(MoveWithDir(Vector2.right));
                    break;
                case Direction.Left:
                    _MoveRoutine.StartRoutine(MoveWithDir(Vector2.left));
                    break;
            }
            if (moveDir9 != DIRECTION9.END) MoveAction(moveDir9);
        }

    }
    private void MoveAction(DIRECTION9 moveDIR9)
    {
        if (mEMove == null)
        {
            mAttackPeriod.StopPeriod();
            Inventory.Instance.ArrackCancel();

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

    private IEnumerator MoveWithDir(Vector3 direction)
    {
        Vector2 movePointMin = Vector2.zero;
        Vector2 movePointMax = Vector2.zero;

        switch (GetLPOSITION3())
        {
            case LPOSITION3.TOP:
                movePointMin = Castle.Instance.GetMovePoint(DIRECTION9.TOP_LEFT);
                movePointMax = Castle.Instance.GetMovePoint(DIRECTION9.TOP_RIGHT);
                break;
            case LPOSITION3.MID:
                movePointMin = Castle.Instance.GetMovePoint(DIRECTION9.MID_LEFT);
                movePointMax = Castle.Instance.GetMovePoint(DIRECTION9.MID_RIGHT);
                break;
            case LPOSITION3.BOT:
                movePointMin = Castle.Instance.GetMovePoint(DIRECTION9.BOT_LEFT);
                movePointMax = Castle.Instance.GetMovePoint(DIRECTION9.BOT_RIGHT);
                break;
        }
        bool IsOutOfRange()
        {
            return transform.position.x < movePointMin.x 
                || transform.position.x > movePointMax.x;
        }
        while (!IsOutOfRange())
        {
            transform.position += direction * Time.deltaTime * Time.timeScale * AbilityTable.MoveSpeed;
            yield return null;
        }
        if (IsOutOfRange())
        {
            Vector2 position = transform.position;
                    position.x = Mathf.Clamp(position.x, movePointMin.x, movePointMax.x);

            transform.position = position;
        }
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
        if (AbilityTable.Table[Ability.CurHealth] <= 0f)
        {
            Debug.Log("저장");

#if UNITY_EDITOR
#else
            BackEndServerManager.Instance.SendDataToServerSchema("Player");
#endif
            DeathAction();
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
