using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField]
    private string[] mTargetTags;

    private Action<ICombat> mTriggerAction;

    public void SetTriggerAction(Action<ICombat> action)
    {
        mTriggerAction = action;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        for (int i = 0; i < mTargetTags.Length; ++i)
        {
            if (collision.CompareTag(mTargetTags[i]) && collision.TryGetComponent(out ICombat combat))
            {
                mTriggerAction.Invoke(combat);

            }
        }
    }
}
