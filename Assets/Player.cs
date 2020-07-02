using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IObject
{
    private DIRECTION9 mLocation9;

    private IEnumerator mCRmove;

    [SerializeField]
    private Golden_Flip mGolden_Flip;

    private void Start()
    {
        mLocation9 = DIRECTION9.MID;
    }

    private void Update()
    {
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

        while (value < 1)
        {
            temporary = value + Time.deltaTime * 5.5f;

            value = temporary > 1 ? 1 : temporary;

            transform.position = Vector2.Lerp(initPos, movePoint, value);

            yield return null;
        }
        mCRmove = null;
        yield break;
    }

    void IObject.IInit() { }
    void IObject.IUpdate() { }
}
