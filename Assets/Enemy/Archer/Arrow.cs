using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField]
    private string[] mTargetTags;

    private Action<ICombat> mTriggerAction;
    private Func<uint, bool> mCanDestroy;

    private uint mEnterCount;

    public void SetTriggerAction(Action<ICombat> action)
    {
        mTriggerAction = action;
    }

    public void SetDestroyCondition(Func<uint, bool> canDestroy)
    {
        mCanDestroy = canDestroy;
    }

    private void OnEnable()
    {
        mEnterCount = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        for (int i = 0; i < mTargetTags.Length; ++i)
        {
            if (collision.TryGetComponent(out ICombat combat))
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
