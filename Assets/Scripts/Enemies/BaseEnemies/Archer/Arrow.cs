using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            if (mCanDestroy.Invoke(mEnterCount))
            {
                gameObject.SetActive(false); yield break;
            }
            transform.position += (Vector3)(DeltaTime * mDirection * mSpeed);

            yield return null;
        }
    }

    private void OnEnable()
    {
        mEnterCount = 0; 
        mCanDestroy = mCanDestroy ?? (o => false);

        StartCoroutine(EUpdate());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (mTargetTags.Any(o => collision.CompareTag(o)))
        {
            if (collision.TryGetComponent(out ICombatable combat))
            {
                mTriggerAction?.Invoke(combat); mEnterCount++;
            }
        }
    }
}
