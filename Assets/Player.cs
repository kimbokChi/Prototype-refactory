using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IObject, ICombat
{
    public Detector EDetector;

    private SpriteRenderer mRenderer;

    private DIRECTION9 mLocation9;

    private IEnumerator mCRmove;

    public  float WaitTimeATK;
    private float mWaitSumATK = 0.0f;

    [SerializeField]
    private Item mEquipItem;

    private void Start()
    {
        mLocation9 = DIRECTION9.MID;

        mEquipItem.Init();

        TryGetComponent(out mRenderer);
    }

    private void Update()
    {
        EDetector.SetRange(mEquipItem.WeaponRange);

        Collider2D challenger = EDetector.GetChallenger();

        if (challenger)
        {
            if (WaitATK())
            {
                mEquipItem.UseItem(ITEM_KEYWORD.STRUCK);
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

    private bool WaitATK()
    {
        mWaitSumATK += Time.deltaTime;

        if (mWaitSumATK >= WaitTimeATK)
        {
            mWaitSumATK = 0.0f; return true;
        }
        return false;
    }

    void IObject.IInit() { }
    void IObject.IUpdate() { }

    public void Damaged(float damage, GameObject attacker, out GameObject victim)
    {
        victim = gameObject;
    }
}
