using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IObject, ICombatable
{
    [SerializeField] protected float mWaitForMoveMin;
    [SerializeField] protected float mWaitForMoveMax;

    [SerializeField] protected float mHalfMoveRangeX;
    [SerializeField] protected float mHalfMoveRangeY;

    [SerializeField] protected float mWaitATKTime;

    [SerializeField] protected float mRange;

    [SerializeField] protected Vector2 mOriginPosition;

    [SerializeField][Range(0.01f, 1f)] protected float mMoveSmooth;

    [SerializeField][Range(0.00f, 1f)] protected float mRangeOffset;

    protected SpriteRenderer mRenderer;

    protected bool  IsMoveFinish => mIsMoveFinish;
    private   bool mIsMoveFinish = true;

    protected float WaitMoveTime
    {
        get => Random.Range(mWaitForMoveMin, mWaitForMoveMax);
    }

    protected Player mPlayer;

    private IEnumerator mEMove;

    #region MEMBER
    /// <summary>
    /// lookingDirection값을 통해서 플레이어를 바라보고 있는지의 여부만을 반환합니다.
    /// </summary>
    #endregion
    protected bool CanLookAtPlayer(Vector2 lookingDirection)
    {
        if (mPlayer != null)
        {
            if (mRenderer == null)
            {
                Debug.Assert(TryGetComponent(out mRenderer));
            }

            // LOOK AT THE LEFT
            if (lookingDirection.x < 0)
            {
                if (PositionLocalized(mPlayer.transform.position).x < transform.position.x)
                {
                    return true;
                }
            }

            // LOOK AT THE RIGHT
            else if (lookingDirection.x > 0)
            {
                if (PositionLocalized(mPlayer.transform.position).x > transform.position.x)
                {
                    return true;
                }
            }
        }
        return false;
    }

    #region MEMBER
    /// <summary>
    /// 해당 개체의 Sprite Flip값을 통해서 플레이어를 바라보고 있는지의 여부를 반환합니다.
    /// <para>
    /// 만약 플레이어를 바라보고 있다면, 플레이어의 위치를 out메서드로 반환합니다.
    /// </para>
    /// </summary>
    #endregion
    protected bool IsLookAtPlayer(out Vector2 playerPos)
    {
        if (mPlayer != null)
        {
            playerPos = PositionLocalized(mPlayer.transform.position);

            if (mRenderer == null)
            {
                Debug.Assert(TryGetComponent(out mRenderer));
            }

            // LOOK AT THE LEFT
            if (mRenderer.flipX)
            {
                if (playerPos.x < transform.position.x)
                {
                    return true;
                }
            }

            // LOOK AT THE RIGHT
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
    /// 해당 개체의 Sprite Flip값을 통해서 플레이어를 바라보고 있는지의 여부만을 반환합니다.
    /// </summary>
    #endregion
    protected bool IsLookAtPlayer()
    {
        if (mPlayer != null)
        {
            if (mRenderer == null)
            {
                Debug.Assert(TryGetComponent(out mRenderer));
            }

            // LOOK AT THE LEFT
            if (mRenderer.flipX)
            {
                if (PositionLocalized(mPlayer.transform.position).x < transform.position.x)
                {
                    return true;
                }
            }

            // LOOK AT THE RIGHT
            else
            {
                if (PositionLocalized(mPlayer.transform.position).x > transform.position.x)
                {
                    return true;
                }
            }
        }
        return false;
    }

    #region MEMBER
    /// <summary>
    /// 인자로 지정한 월드 좌표를, 해당 개체의 로컬 좌표를 기준으로 변환한 값을 반환합니다.
    /// </summary>
    #endregion
    protected Vector2 PositionLocalized(Vector2 positon)
    {
        return transform.InverseTransformPoint(positon) + transform.localPosition;
    }

    #region MEMBER
    /// <summary>
    /// 인자로 지정한 지점이 해당 개체의 사정거리안에 들어와 있는지를 반환합니다.
    /// </summary>
    #endregion
    protected bool IsPointOnRange(Vector2 point)
    {
        return (Vector2.Distance(point, transform.localPosition) <= mRange + mRangeOffset);
    }

    protected bool IsInReachPlayer()
    {
        if (mPlayer != null)
        {
            if (IsLookAtPlayer(out Vector2 playerPoint))
            {
                return IsPointOnRange(playerPoint);
            }
        }
        return false;
    }

    #region MEMBER
    /// <summary>
    /// 인자로 지정한 지점을 향해 이동합니다. 그리고 이 이동이 끝나게되면 MoveFinish함수를 호출합니다.
    /// <para>
    /// 만약 이동중에 이 함수가 호출되었다면, 진행중이던 이동을 중단하고 새로 호출한 값을 바탕으로 이동합니다.
    /// </para>
    /// </summary>
    #endregion
    protected void MoveToPoint(Vector2 point)
    {
        if (mEMove != null)
        {
            StopCoroutine(mEMove);
        }
        if (mRenderer == null)
        {
            Debug.Assert(TryGetComponent(out mRenderer));
        }
        mRenderer.flipX = point.x < transform.localPosition.x;

        StartCoroutine(mEMove = EMove(point));
    }

    #region EVENT
    /// <summary>
    /// MoveToPoint함수를 통한 해당 개체의 이동이 끝날때, 호출되는 함수입니다.
    /// </summary>
    #endregion
    protected virtual void MoveFinish() { }

    protected void MoveStop()
    {
        if (mEMove != null)
        {
            StopCoroutine(mEMove);

            MoveStopEvent();
        }
    }

    private IEnumerator EMove(Vector2 movePoint)
    {
        Vector2 refVelocity = Vector2.zero;

        mIsMoveFinish = false;

        movePoint = FitToMoveArea(movePoint);

        while (Vector2.Distance(movePoint, transform.localPosition) > mMoveSmooth)
        {
            float deltaTime = Time.deltaTime * Time.timeScale;

            float moveSpeed = GetAbility.MoveSpeed + GetAbility.IMoveSpeed;

            transform.localPosition = Vector2.SmoothDamp(transform.localPosition, movePoint, ref refVelocity, 0.5f, moveSpeed, deltaTime);

            yield return null;
        }
        MoveStopEvent();

        yield break;
    }

    private void MoveStopEvent()
    {
        mEMove = null;

        mIsMoveFinish = true;

        MoveFinish();
    }

    public Vector2 FitToMoveArea(Vector2 vector)
    {
        for (int i = 0; i < 2; i++)
        {
            float element = i.Equals(0) ? vector.x : vector.y;

            if (element < -mHalfMoveRangeX + mOriginPosition.x)
            {
                element = -mHalfMoveRangeX + mOriginPosition.x;
            }
            else
            {
                if (element > mHalfMoveRangeX + mOriginPosition.x)
                {
                    element = mHalfMoveRangeX + mOriginPosition.x;
                }
            }
            if (i.Equals(0)) vector.x = element;
            else             vector.y = element;
        }
        return vector;
    }

    #region interfaces : 
    public abstract AbilityTable GetAbility { get; }

    public abstract void IInit();
    public abstract bool IsActive();
    public abstract void IUpdate();
    public abstract void PlayerEnter(MESSAGE message, Player enterPlayer);
    public abstract void PlayerExit (MESSAGE message);
    public abstract GameObject ThisObject();
    public abstract void Damaged(float damage, GameObject attacker);
    public abstract void CastBuff(BUFF buffType, IEnumerator castedBuff);
    #endregion
}
