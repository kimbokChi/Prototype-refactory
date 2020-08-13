using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scarecrow : MonoBehaviour, IObject, ICombat
{
    [SerializeField] private float mWaitForMoveMin;
    [SerializeField] private float mWaitForMoveMax;

    [SerializeField] private float mHalfMoveRangeX;
    [SerializeField] private float mHalfMoveRangeY;

    private float mWaitMoveTime
    {
        get
        {
            return Random.Range(mWaitForMoveMin, mWaitForMoveMax);
        }
    }

    [SerializeField]
    private float mWaitATKTime;

    [SerializeField]
    private float mRange;

    [SerializeField][Range(0.01f, 1f)]
    private float mMoveSmooth;

    [SerializeField][Range(0f, 1f)]
    private float mRangeOffset;

    [SerializeField]
    private float mMoveSpeed;

    [SerializeField][Tooltip("해당 개체가 한번 움직일 때마다 이동에 걸리는 시간을 지정합니다. 시간이 적을수록 더욱 빠르게 움직입니다.")]
    private float mMoveTime;

    [SerializeField]
    private float mMaxHealth;
    private float mCurHealth;

    private int mLocateFloor;

    private Timer mWaitForATK;
    private Timer mWaitForMove;

    private Player mPlayer;

    private Vector2 mOriginPositon;

    private SpriteRenderer mRenderer;

    private IEnumerator mEMove;
    private IEnumerator EMove(Vector2 movePoint)
    {
        Vector2 refVelocity = Vector2.zero;

        movePoint = ControlMovePoint(movePoint);

        while (Vector2.Distance(movePoint, transform.localPosition) > mMoveSmooth)
        {
            float deltaTime = Time.deltaTime * Time.timeScale;

            transform.localPosition = Vector2.SmoothDamp(transform.localPosition, movePoint, ref refVelocity, 0.5f, mMoveSpeed, deltaTime);

            yield return null;
        }
        mEMove = null;

        mWaitForMove.Start(mWaitMoveTime); 

        yield break;
    }

    public void IInit()
    {
        mWaitForATK  = new Timer();
        mWaitForATK.Start(mWaitATKTime);

        mWaitForMove = new Timer();
        mWaitForMove.Start(mWaitMoveTime);

        if (transform.parent.TryGetComponent(out Room room))
        {
            mLocateFloor = room.BelongFloorIndex;
        }
        TryGetComponent(out mRenderer);

        mCurHealth = mMaxHealth;

        mOriginPositon = transform.localPosition;
    }
    public void IUpdate()
    {
        if (mWaitForMove.IsOver())
        {
            if (mEMove == null)
            {
                Vector2 movePoint;
                Vector2 playerPoint;

                movePoint.x = Random.Range(-mHalfMoveRangeX, mHalfMoveRangeX);
                movePoint.y = Random.Range(-mHalfMoveRangeY, mHalfMoveRangeY) + mOriginPositon.y;

                if (!IsLookAtPlayer(out playerPoint))
                {
                    mRenderer.flipX = (movePoint.x < transform.localPosition.x);
                }
                if (IsLookAtPlayer(out playerPoint))
                {
                    movePoint = playerPoint;

                    if (!IsRangeInPoint(movePoint))
                    {
                        movePoint -= (mRenderer.flipX ? Vector2.left : Vector2.right) * mRange;
                    }
                }
                StartCoroutine(mEMove = EMove(movePoint));
            }
        }
        else
        {
            mWaitForMove.Update();
        }

        if (mPlayer != null)
        {
            if (mWaitForATK.IsOver())
            {
                if (IsLookAtPlayer(out Vector2 p) && IsRangeInPoint(PlayerLocalized(), mRangeOffset))
                {
                    mPlayer.Damaged(1f, gameObject, out GameObject v);

                    mWaitForATK.Start(mWaitATKTime);
                }
            }
            else
            {
                mWaitForATK.Update();
            }
        }
    }

    public void PlayerEnter(Player enterPlayer)
    {
        mPlayer = enterPlayer;

        Debug.Log("Player Enter");
    }

    public void PlayerExit()
    {
        mPlayer = null;

        mWaitForATK.Start(mWaitATKTime);

        Debug.Log("Player Exit");
    }

    private bool IsLookAtPlayer(out Vector2 playerPos)
    {
        if (mPlayer != null)
        {
            playerPos = PlayerLocalized();

            // LOOK AT THE LEFT
            if (mRenderer.flipX)
            {
                if (playerPos.x < transform.position.x)
                {
                    return true;
                }
            }

            // LOOL AT THE RIGHT
            else
            {
                if (playerPos.x > transform.position.x)
                {
                    return true;
                }
            }
        }
        playerPos = Vector2.zero;

        return false;
    }

    #region MEMBER
    /// <summary>
    /// 해당 개체의 사정거리 내에 localizePoint가 존재하는지의 여부를 반환합니다.
    /// </summary>
    #endregion
    private bool IsRangeInPoint(Vector2 localizePoint, float rangeOffset = 0f)
    {
        return (Vector2.Distance(localizePoint, transform.localPosition) <= mRange + rangeOffset);
    }

    #region MEMBER
    /// <summary>
    /// 해당 개체의 지역 좌표를 기준으로한 플레이어의 좌표를 반환합니다.
    /// <para>플레이어 개체가 확실히 존재할 때에만 사용하십시오.</para>
    /// </summary>
    #endregion
    private Vector2 PlayerLocalized()
    {
        return transform.InverseTransformPoint(mPlayer.transform.position) + transform.localPosition;
    }

    private Vector2 ControlMovePoint(Vector2 movePoint)
    {
        if (mHalfMoveRangeY == 0)
        {
            movePoint.y = transform.localPosition.y;
        }
        else if (Mathf.Abs(movePoint.y - mOriginPositon.y) > mHalfMoveRangeY)
        {
            movePoint.y = mHalfMoveRangeY + mOriginPositon.y;
        }

        if (mHalfMoveRangeX == 0)
        {
            movePoint.x = transform.localPosition.x;
        }
        else if (Mathf.Abs(movePoint.x - mOriginPositon.x) > mHalfMoveRangeX)
        {
            movePoint.x = mHalfMoveRangeX + mOriginPositon.x;
        }
        return movePoint;
    }

    public void Damaged(float damage, GameObject attacker, out GameObject victim)
    {
        victim = gameObject;

        mCurHealth -= damage;

        if (mCurHealth <= 0) gameObject.SetActive(false);
    }

    public bool IsActive() => gameObject.activeSelf;

    public GameObject ThisObject() => gameObject;
}
