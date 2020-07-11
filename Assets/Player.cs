using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IObject, ICombat
{
    private const float BLINK_TIME = 0.5f;
    private Timer mBlinkTimer = new Timer();

    public float WaitTimeATK;
    private Timer mWaitATK = new Timer();


    public Detector EDetector;

    private SpriteRenderer mRenderer;

    private DIRECTION9 mLocation9;

    private IEnumerator mCRmove;

    private float mHealthPoint = 100.0f;
    private float mDefensivePower = 1.0f;

    [SerializeField]
    private Item mEquipItem;

    public POSITION3 GetPOSITION9()
    {
        switch (mLocation9)
        {
            case DIRECTION9.TOP_LEFT:
            case DIRECTION9.TOP:
            case DIRECTION9.TOP_RIGHT:
                return POSITION3.TOP;

            case DIRECTION9.MID_LEFT:
            case DIRECTION9.MID:
            case DIRECTION9.MID_RIGHT:
                return POSITION3.MID;

            case DIRECTION9.BOT_LEFT:
            case DIRECTION9.BOT:
            case DIRECTION9.BOT_RIGHT:
                return POSITION3.BOT;

            default:
                break;
        }
        Debug.Log("Value Error");
        return POSITION3.NONE;
    }

    private void Start()
    {
        mLocation9 = DIRECTION9.MID;

        mEquipItem.Init();

        TryGetComponent(out mRenderer);

        mWaitATK.Start(WaitTimeATK);
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
                mEquipItem.UseItem(ITEM_KEYWORD.STRUCK);

                mWaitATK.Start(WaitTimeATK);
            }
            mRenderer.flipX = (challenger.transform.position.x > transform.position.x);
        }

        if (mCRmove == null && Input.anyKeyDown)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                mLocation9 = ((int)mLocation9 + 3) > 8 ? mLocation9 : mLocation9 + 3;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                mLocation9 = ((int)mLocation9 - 3) < 0 ? mLocation9 : mLocation9 - 3;
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                mLocation9 = (int)mLocation9 % 3 == 0 ? mLocation9 : mLocation9 - 1;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                mLocation9 = (int)mLocation9 % 3 == 2 ? mLocation9 : mLocation9 + 1;
            }
            mCRmove = CR_move(Castle.Instnace.GetMovePoint(mLocation9));

            StartCoroutine(mCRmove);
        }
    }

    private IEnumerator CR_move(Vector2 movePoint)
    {
        float value = 0;
        float temporary = 0;

        Vector2 initPos = transform.position;

        mEquipItem.UseItem(ITEM_KEYWORD.MOVE_BEGIN);

        while (value < 1)
        {
            temporary = value + Time.deltaTime * 5.5f;

            value = temporary > 1 ? 1 : temporary;

            transform.position = Vector2.Lerp(initPos, movePoint, value);

            yield return null;
        }
        mCRmove = null;
        mEquipItem.UseItem(ITEM_KEYWORD.MOVE_END);

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

        mEquipItem.UseItem(ITEM_KEYWORD.BE_DAMAGED);

        mBlinkTimer.Start(BLINK_TIME);
    }
}
