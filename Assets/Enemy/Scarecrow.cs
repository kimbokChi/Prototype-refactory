using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scarecrow : MonoBehaviour, IObject
{
    [SerializeField] private float WAIT_FOR_MOVE_MIN;
    [SerializeField] private float WAIT_FOR_MOVE_MAX;

    [SerializeField] private float HALF_MOVE_RANGE_X;
    [SerializeField] private float HALF_MOVE_RANGE_Y;

    private float mWaitMoveTime
    {
        get
        {
            return Random.Range(WAIT_FOR_MOVE_MIN, WAIT_FOR_MOVE_MAX);
        }
    }

    [SerializeField]
    private float mMoveSpeed;

    [SerializeField][Tooltip("해당 개체가 한번 움직일 때마다 이동에 걸리는 시간을 지정합니다. 시간이 적을수록 더욱 빠르게 움직입니다.")]
    private float mMoveTime;

    private int mLocateFloor;

    private Timer mWaitForMove;

    private Player mPlayer;

    private SpriteRenderer mRenderer;

    private IEnumerator mEMove;
    private IEnumerator EMove(Vector2 movePoint)
    {
        Vector2 refVelocity = Vector2.zero;

        while (Vector2.Distance(transform.localPosition, movePoint) > 0.5f)
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
        mWaitForMove = new Timer();

        mWaitForMove.Start(mWaitMoveTime);

        if (transform.parent.TryGetComponent(out Room room))
        {
            mLocateFloor = room.BelongFloorIndex;
        }
        TryGetComponent(out SpriteRenderer renderer);
    }
    public void IUpdate()
    {
        if (mWaitForMove.IsOver())
        {
            if (mEMove == null)
            {
                Vector2 movePoint;

                movePoint.x = Random.Range(-HALF_MOVE_RANGE_X, HALF_MOVE_RANGE_X);
                movePoint.y = Random.Range(-HALF_MOVE_RANGE_Y, HALF_MOVE_RANGE_Y) + transform.localPosition.y;

                mRenderer.flipX = (movePoint.x < transform.localPosition.x);

                StartCoroutine(mEMove = EMove(movePoint));
            }
        }
        else
        {
            mWaitForMove.Update();
        }
    }

    public void PlayerEnter()
    {
        mPlayer = FindObjectOfType(typeof(Player)) as Player;
    }

    public void PlayerExit()
    {
        mPlayer = null;
    }
    private bool IsLookAtPlayer(out Vector2 playerPos)
    {
        if (mPlayer != null)
        {
            playerPos = mPlayer.transform.position;

            // LOOK AT THE LEFT
            if (mRenderer.flipX)
            {
                if (mPlayer.transform.position.x < transform.position.x)
                {
                    return true;
                }
            }

            // LOOL AT THE RIGHT
            else
            {
                if (mPlayer.transform.position.x > transform.position.x)
                {
                    return true;
                }
            }
        }
        playerPos = Vector2.zero;

        return false;
    }

}
