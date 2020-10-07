using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, ICombatable
{
    [SerializeField]
    private GameObject mGameOverWindow;

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

    public  bool IsDeath => mIsDeath;

    public AbilityTable GetAbility => AbilityTable;

    private bool mIsDeath;

    private bool mCanAttack;

    private CircleCollider2D mRangeCollider;

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
        mIsDeath      = false;

        mIsMovingElevation = false;

        mBlinkTimer = new Timer();

        mAttackPeriod = new AttackPeriod(AbilityTable);
        mAttackPeriod.SetAction(Period.Attack, AttackAction);

        RangeArea.SetEnterAction( o => mCanAttack = true);
        RangeArea.SetEmptyAction(() => mCanAttack = false);

        mCollidersOnMove = new List<Collider2D>();

        DeathEvent += () => mGameOverWindow.SetActive(true);
        DeathEvent += () => RangeArea.enabled = false;

        Debug.Assert(gameObject.TryGetComponent(out mRenderer));

        mInventory = Inventory.Instance;

        if (RangeArea.gameObject.TryGetComponent(out mRangeCollider))
        {
            mInventory.WeaponChangeEvent +=  
                o => mRangeCollider.radius = o.WeaponRange;
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

        if (mCanAttack) mAttackPeriod.Update();

        if (!mIsDeath)
        {
            InputAction();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (mEMove != null) mCollidersOnMove.Add(collision);
    }

    private void AttackAction()
    {
        if (RangeArea.TryEnterTypeT(out Transform transform))
        {
            if (transform.TryGetComponent(out ICombatable combat))
            {
                mInventory.OnAttack(gameObject, combat);
            }
            mRenderer.flipX = (transform.position.x > this.transform.position.x);
        }
    }

    private void MoveAction(DIRECTION9 moveDIR9)
    {
        if (mEMove == null && moveDIR9 != mLocation9)
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
                Vector2 movePoint = Castle.Instance.GetMovePoint(moveDIR9);

                StartCoroutine(mEMove = EMove(movePoint, moveDIR9));
            }
        }
    }

    private IEnumerator EMove(Vector2 movePoint, DIRECTION9 moveDIR9)
    {
        mInventory.OnMoveBegin(movePoint.normalized);

        float  lerpAmount = 0f; 
        while (lerpAmount < 1f)
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
            if (mIsDeath = AbilityTable.Table[Ability.CurHealth] <= 0f)
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
