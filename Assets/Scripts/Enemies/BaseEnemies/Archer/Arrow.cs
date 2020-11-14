using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField]
    private string[] mTargetTags;

    [SerializeField]
    private SpriteRenderer SpriteRenderer;

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

        if (direction.x < 0) {
            SpriteRenderer.flipX = false;
        }
        else if (direction.x > 0) {
            SpriteRenderer.flipX = true;
        }
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

    private void Reset()
    {
        Debug.Assert(TryGetComponent(out SpriteRenderer));
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
