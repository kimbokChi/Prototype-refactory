using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, ICombat
{
    [SerializeField]
    private float mBlinkTime;
    private Timer mBlinkTimer;

    [SerializeField]
    private float WaitTimeATK;
    private Timer mWaitATK;

    [SerializeField]
    private float mMoveSpeed;

    [SerializeField] 
    private Detector mEnemyDetector;

    [SerializeField]
    private float mMaxHealth;
    private float mCurHealth;

    [SerializeField]
    private float mDefense;

    [SerializeField]
    private DIRECTION9 mLocation9;


    private IEnumerator mEMove;

    private bool mCanElevation;

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

        mCurHealth = mMaxHealth;

        mWaitATK    = new Timer();
        mBlinkTimer = new Timer();

        mWaitATK.Start(WaitTimeATK);
    }

    private void InputAction()
    {
        DIRECTION9 moveRIR9 = DIRECTION9.END;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            if (GetLPOSITION3() == LPOSITION3.TOP)
            {
                mCanElevation = Castle.Instnace.CanNextPoint();
            }
            if (!mCanElevation)
            {
                moveRIR9 = ((int)mLocation9 - 3) < 0 ? mLocation9 : mLocation9 - 3;
            }
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            moveRIR9 = ((int)mLocation9 + 3) > 8 ? mLocation9 : mLocation9 + 3;
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
        mWaitATK.Update();

        if (!mBlinkTimer.IsOver()) 
        {
            mBlinkTimer.Update(); 
        }

        mEnemyDetector.SetRange(Inventory.Instnace.GetWeaponRange());

        if (mEnemyDetector.HasChallenger(out Collider2D challenger))
        {
            if (mWaitATK.IsOver() && challenger.TryGetComponent(out ICombat combat))
            {
                // Inventory.Instnace.UseItem(ITEM_KEYWORD.STRUCK);

                mWaitATK.Start(WaitTimeATK);
            }
            if (TryGetComponent(out SpriteRenderer renderer))
            {
                renderer.flipX = (challenger.transform.position.x > transform.position.x);
            }            
        }
        InputAction();
    }

    private void MoveAction(DIRECTION9 moveDIR9)
    {
        if (mEMove == null && moveDIR9 != mLocation9)
        {
            // Move To Next Floor
            if (mCanElevation)
            {
                if (Castle.Instnace.CanNextPoint(out Vector2 nextPoint))
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
                StartCoroutine(mEMove = EMove(Castle.Instnace.GetMovePoint(moveDIR9), moveDIR9));
            }
        }
    }

    private IEnumerator EMove(Vector2 movePoint, DIRECTION9 moveDIR9)
    {
        // Inventory.Instnace.UseItem(ITEM_KEYWORD.MOVE_BEGIN);

        float lerpAmount = 0;

        while (lerpAmount < 1)
        {
            lerpAmount = Mathf.Min(1, lerpAmount + Time.deltaTime * Time.timeScale * mMoveSpeed);

            transform.position = Vector2.Lerp(transform.position, movePoint, lerpAmount);

            yield return null;
        }
        // Inventory.Instnace.UseItem(ITEM_KEYWORD.MOVE_END);

        if (mCanElevation)
        {
            Castle.Instnace.AliveNextPoint();

            // Inventory.Instnace.UseItem(ITEM_KEYWORD.ENTER);

            mCanElevation = false;
        }
        mLocation9 = moveDIR9; mEMove = null;

        yield break;
    }

    public void Damaged(float damage, GameObject attacker, out GameObject victim)
    {
        if (!mBlinkTimer.IsOver())
        {
            victim = null; return;
        }
        victim = gameObject;

        Inventory.Instnace.BeDamaged(ref damage, attacker, gameObject);

        mCurHealth -= damage / mDefense;

        mBlinkTimer.Start(mBlinkTime);
    }
}
