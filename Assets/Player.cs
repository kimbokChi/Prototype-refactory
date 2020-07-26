using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class Player : MonoBehaviour, IObject, ICombat
{
    private const float BLINK_TIME = 0.5f;
    private Timer mBlinkTimer = new Timer();

    public float WaitTimeATK;
    private Timer mWaitATK = new Timer();

    public Detector EDetector;
    public Inventory mInventory;

    private UInputAction mInputSystem;

    private SpriteRenderer mRenderer;

    private DIRECTION9 mLocation9;

    private IEnumerator mCRmove;

    private float mHealthPoint = 100.0f;
    private float mDefensivePower = 1.0f;

    private bool mCanElevation = false;

    [SerializeField]
    private Item mEquipItem;

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
        mLocation9 = DIRECTION9.MID;

        mInventory.Init();

        mEquipItem.Init();

        TryGetComponent(out mRenderer);

        mWaitATK.Start(WaitTimeATK);

        mInputSystem = new UInputAction();
        
        mInputSystem.PlayerControl.Input.performed += InputAct;

        mInputSystem.Enable();
    }

    private void InputAct(InputAction.CallbackContext context)
    {
        DIRECTION9 moveRIR9 = DIRECTION9.END;

        // Up Arrow
        if (context.control.path.Equals(Keyboard.current.upArrowKey.path))
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

        // Dwon Arrow
        if (context.control.path.Equals(Keyboard.current.downArrowKey.path))
        {
            moveRIR9 = ((int)mLocation9 + 3) > 8 ? mLocation9 : mLocation9 + 3;
        }

        // Left Arrow
        if (context.control.path.Equals(Keyboard.current.leftArrowKey.path))
        {
            moveRIR9 = (int)mLocation9 % 3 == 0 ? mLocation9 : mLocation9 - 1;
        }

        // Right Arrow
        if (context.control.path.Equals(Keyboard.current.rightArrowKey.path))
        {
            moveRIR9 = (int)mLocation9 % 3 == 2 ? mLocation9 : mLocation9 + 1;
        }

        if (moveRIR9 != DIRECTION9.END)
        {
            MoveAction(moveRIR9);
        }
    }

    private void Update()
    {
        EDetector.SetRange(mEquipItem.WeaponRange);

        Collider2D challenger = EDetector.GetChallenger();

        mWaitATK.Update();

        if (!mBlinkTimer.IsOver()) { mBlinkTimer.Update(); }

        if (challenger)
        {
            if (mWaitATK.IsOver())
            {
                mInventory.UseItem(ITEM_KEYWORD.STRUCK);

                mWaitATK.Start(WaitTimeATK);
            }
            mRenderer.flipX = (challenger.transform.position.x > transform.position.x);
        }
    }

    private void MoveAction(DIRECTION9 moveRIR9)
    {
        if (mCRmove == null)
        {
            if (mCanElevation)
            {
                Vector2 nextPoint;

                if (Castle.Instnace.CanNextPoint(out nextPoint))
                {
                    mCRmove = CR_move(nextPoint);
                }
            }
            else mCRmove = CR_move(Castle.Instnace.GetMovePoint(moveRIR9));

            if (mCRmove != null)
            {
                mLocation9 = moveRIR9;

                StartCoroutine(mCRmove);
            }
        }
    }

    private IEnumerator CR_move(Vector2 movePoint)
    {
        float value = 0;
        float temporary = 0;

        Vector2 initPos = transform.position;

        mInventory.UseItem(ITEM_KEYWORD.MOVE_BEGIN);

        while (value < 1)
        {
            temporary = value + Time.deltaTime * 5.5f;

            value = temporary > 1 ? 1 : temporary;

            transform.position = Vector2.Lerp(initPos, movePoint, value);

            yield return null;
        }
        mCRmove = null;
        mInventory.UseItem(ITEM_KEYWORD.MOVE_END);

        if (mCanElevation)
        {
            switch (mLocation9)
            {
                case DIRECTION9.TOP_LEFT:
                    mLocation9 = DIRECTION9.BOT_LEFT;
                    break;
                case DIRECTION9.TOP:
                    mLocation9 = DIRECTION9.BOT;
                    break;
                case DIRECTION9.TOP_RIGHT:
                    mLocation9 = DIRECTION9.BOT_RIGHT;
                    break;
            }
            Castle.Instnace.AliveNextPoint();
            mCanElevation = false;
        }
        yield break;
    }

    void IObject.IInit() { }
    void IObject.IUpdate() { }

    public void Damaged(float damage, GameObject attacker, out GameObject victim)
    {
        // 깜박이 상태라면 공격 무시
        if (!mBlinkTimer.IsOver())
        { victim = null; Debug.Log("Blink!"); return; }

        victim = gameObject;

        mHealthPoint -= damage / mDefensivePower;

        mInventory.UseItem(ITEM_KEYWORD.BE_DAMAGED);

        mBlinkTimer.Start(BLINK_TIME);
    }
}
