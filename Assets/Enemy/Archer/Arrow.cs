using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField]
    private string[] mTargetTags;

    private Action<ICombatable> mTriggerAction;
    private Func<uint, bool> mCanDestroy;

    private uint mEnterCount;

    private Vector2 mDirection;
    private float mSpeed;

    private float DeltaTime
    { get => Time.deltaTime * Time.timeScale; }

    public void Setting(Action<ICombatable> targetHit, Func<uint, bool> canDestroy)
    {
        mCanDestroy = canDestroy; mTriggerAction = targetHit;
    }

    public void Setting(float speed, Vector2 direction)
    {
        mDirection = direction; mSpeed = speed;
    }

    private IEnumerator EUpdate()
    {
        while (gameObject.activeSelf)
        {
            if (mCanDestroy != null)
            {
                if (mCanDestroy.Invoke(mEnterCount))
                {
                    gameObject.SetActive(false);

                    yield break;
                }
                
            }
            transform.position += (Vector3)(DeltaTime * mDirection * mSpeed);

            yield return null;
        }
    }

    private void OnEnable()
    {
        mEnterCount = 0;

        StartCoroutine(EUpdate());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        for (int i = 0; i < mTargetTags.Length; ++i)
        {
            if (collision.TryGetComponent(out ICombatable combat))
            {
                if (collision.CompareTag(mTargetTags[i]))
                {
                    mTriggerAction.Invoke(combat);

                    mEnterCount++;
                }
            }
        }
    }
}
