using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scarecrow : MonoBehaviour, IObject
{
    private const float WAIT_FOR_MOVE = 1.2f;

    private const float HALF_MOVE_RANGE_X = 2.5f;
    private const float HALF_MOVE_RANGE_Y = 0.0f;

    [SerializeField]
    private float mMoveSpeed;

    private Timer mWaitForMove;

    private IEnumerator mEMove;

    private void Start()
    {
        mWaitForMove = new Timer();

        mWaitForMove.Start(WAIT_FOR_MOVE);
    }

    private void Update()
    {
        if (mWaitForMove.IsOver())
        {
            if (mEMove == null)
            {
                Vector2 movePoint;

                movePoint.x = Random.Range(-HALF_MOVE_RANGE_X, HALF_MOVE_RANGE_X);
                movePoint.y = Random.Range(-HALF_MOVE_RANGE_Y, HALF_MOVE_RANGE_Y) + transform.position.y;

                if (TryGetComponent(out SpriteRenderer renderer))
                {
                    renderer.flipX = (movePoint.x < transform.position.x);
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

        Vector2 beginPos = transform.position;

        while (lerp < 1)
        {
            lerp = Mathf.Min(lerp + Time.deltaTime * Time.timeScale * mMoveSpeed, 1);

            transform.position = Vector2.Lerp(beginPos, movePoint, lerp);

            yield return null;
        }
        mEMove = null;

        mWaitForMove.Start(WAIT_FOR_MOVE); 

        yield break;
    }

    public void IInit()
    {

    }
    public void IUpdate()
    {
        
    }
}
