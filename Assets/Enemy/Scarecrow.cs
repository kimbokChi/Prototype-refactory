using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scarecrow : MonoBehaviour, IObject
{
    private const float WAIT_FOR_MOVE_MIN = 0.6f;
    private const float WAIT_FOR_MOVE_MAX = 1.2f;

    private const float HALF_MOVE_RANGE_X = 2.5f;
    private const float HALF_MOVE_RANGE_Y = 0.0f;

    private float mWaitMoveTime
    {
        get
        {
            return Random.Range(WAIT_FOR_MOVE_MIN, WAIT_FOR_MOVE_MAX);
        }
    }

    [SerializeField]
    private float mMoveSpeed;

    [SerializeField]
    private float mMaxVelocity;

    private Timer mWaitForMove;

    private IEnumerator mEMove;

    private void Start()
    {
        mWaitForMove = new Timer();

        mWaitForMove.Start(mWaitMoveTime);
    }

    private void Update()
    {
        if (mWaitForMove.IsOver())
        {
            if (mEMove == null)
            {
                Vector2 movePoint;

                movePoint.x = Random.Range(-HALF_MOVE_RANGE_X, HALF_MOVE_RANGE_X);
                movePoint.y = Random.Range(-HALF_MOVE_RANGE_Y, HALF_MOVE_RANGE_Y) + transform.localPosition.y;

                if (TryGetComponent(out SpriteRenderer renderer))
                {
                    renderer.flipX = (movePoint.x < transform.localPosition.x);
                }
                StartCoroutine(mEMove = EMove(movePoint));
            }
        }
        else
        {
            mWaitForMove.Update();
        }
    }

    private IEnumerator EMove(Vector2 movePoint)
    {
        float lerp = 0.0f;

        Vector2 beginPos = transform.localPosition;

        while (lerp < 1)
        {
            lerp = Mathf.Min(lerp + Time.deltaTime * Time.timeScale * mMoveSpeed, 1);

            Vector2 lerpVector = Vector2.Lerp(transform.localPosition, movePoint, lerp);

            if (mMaxVelocity < lerpVector.magnitude)
            {
                lerpVector = lerpVector.normalized * mMaxVelocity;
            }
            transform.localPosition = lerpVector;

            yield return null;
        }
        mEMove = null;

        mWaitForMove.Start(mWaitMoveTime); 

        yield break;
    }

    public void IInit()
    {

    }
    public void IUpdate()
    {
        
    }
}
