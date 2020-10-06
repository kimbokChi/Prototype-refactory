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

    private List<Collider2D> mCollidersOnMove;

    private AttackPeriod mAttackPeriod;

    private event Action DeathEvent;

    private bool mCanElevation;

    private bool mIsMoveToUpDown;

    public  bool IsDeath => mIsDeath;

    public AbilityTable GetAbility => AbilityTable;

    private bool mIsDeath;

    private bool mCanAttack;

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

        mIsMoveToUpDown = false;

        mBlinkTimer = new Timer();

        mAttackPeriod = new AttackPeriod(AbilityTable);
        mAttackPeriod.SetAction(Period.Attack, AttackAction);

        RangeArea.SetEnterAction( o => mCanAttack = true);
        RangeArea.SetEmptyAction(() => mCanAttack = false);

        mCollidersOnMove = new List<Collider2D>();

        DeathEvent += () => mGameOverWindow.SetActive(true);
        DeathEvent += () => RangeArea.enabled = false;
    }

    private void InputAction()
    {
        DIRECTION9 moveRIR9 = DIRECTION9.END;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            if (GetLPOSITION3() == LPOSITION3.TOP)
            {
                mCanElevation = Castle.Instance.CanNextPoint();
            }
            if (!mCanElevation)
            {
                DIRECTION9 prevLocation9 = mLocation9;

                moveRIR9 = ((int)mLocation9 - 3) < 0 ? mLocation9 : mLocation9 - 3;

                mIsMoveToUpDown = (prevLocation9 != moveRIR9);
            }
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            DIRECTION9 prevLocation9 = mLocation9;

            moveRIR9 = ((int)mLocation9 + 3) > 8 ? mLocation9 : mLocation9 + 3;

            mIsMoveToUpDown = (prevLocation9 != moveRIR9);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveRIR9 = (int)mLocation9 % 3 == 0 ? mLocation9 : mLocation9 - 1;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            moveRIR9 = (int)mLocation9 % 3 == 2 ? mLocation9 : mLocation9 + 1;
        }

        MoveAction(moveRIR9);
    }
    private void Update()
    {
        if (!mBlinkTimer.IsOver()) 
        {
            mBlinkTimer.Update(); 
        }

        if (mCanAttack) mAttackPeriod.Update();

        if (!mIsDeath) InputAction();
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
                Inventory.Instance.OnAttack(gameObject, combat);
            }
            if (TryGetComponent(out SpriteRenderer renderer))
            {
                renderer.flipX = (transform.position.x > this.transform.position.x);
            }
        }
    }

    private void MoveAction(DIRECTION9 moveDIR9)
    {
        if (mEMove == null && moveDIR9 != mLocation9)
        {
            // Move To Next Floor
            if (mCanElevation)
            {
                if (Castle.Instance.CanNextPoint(out Vector2 nextPoint))
                {
                    switch (mLocation9)
                    {
                        case DIRECTION9.TOP_LEFT:
                            moveDIR9 = DIRECTION9.BOT_LEFT;
                            break;
                        case DIRECTION9.TOP:
                            moveDIR9 = DIRECTION9.BOT;
                            break;
                        case DIRECTION9.TOP_RIGHT:
                            moveDIR9 = DIRECTION9.BOT_RIGHT;
                            break;
                    }
                    StartCoroutine(mEMove = EMove(nextPoint, moveDIR9));
                }
            }

            // Move To MovePoint
            else if (moveDIR9 != DIRECTION9.END)
            {
                StartCoroutine(mEMove = EMove(Castle.Instance.GetMovePoint(moveDIR9), moveDIR9));
            }
        }
    }

    private IEnumerator EMove(Vector2 movePoint, DIRECTION9 moveDIR9)
    {
        Inventory.Instance.OnMoveBegin(movePoint.normalized);

        float lerpAmount = 0;

        while (lerpAmount < 1)
        {
            lerpAmount = Mathf.Min(1, lerpAmount + Time.deltaTime * Time.timeScale * AbilityTable.MoveSpeed);

            transform.position = Vector2.Lerp(transform.position, movePoint, lerpAmount);

            yield return null;
        }
        Inventory.Instance.OnMoveEnd(mCollidersOnMove.ToArray());

        mCollidersOnMove.Clear();

        if (mCanElevation)
        {
            // Inventory.Instance.UseItem(ITEM_KEYWORD.ENTER);

            mCanElevation = false;
        }
        mLocation9 = moveDIR9; mEMove = null;

        mIsMoveToUpDown = false;

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
    public bool Position(out Vector2 playerPos)
    {
        playerPos = transform.position;

        return !mIsMoveToUpDown;
    }

    public void Damaged(float damage, GameObject attacker)
    {
        if (mBlinkTimer.IsOver())
        {
            mBlinkTimer.Start(mBlinkTime);

            Inventory.Instance.OnDamaged(ref damage, attacker, gameObject);

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
