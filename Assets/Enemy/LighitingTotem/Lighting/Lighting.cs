using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lighting : MonoBehaviour
{
    [SerializeField] private float mDamage;
    [SerializeField] private float mDurate;

    [SerializeField] private string[] mTargetTags;

    private Timer mTimer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        for (int i = 0; i < mTargetTags.Length; ++i)
        {
            if (collision.CompareTag(mTargetTags[i]))
            {
                if (collision.TryGetComponent(out ICombatable combat))
                {
                    combat.Damaged(mDamage, gameObject);
                }
            }
        }
    }

    private void OnEnable()
    {
        if (mTimer == null)
        {
            mTimer = new Timer();
        }
        mTimer.Start(mDurate);
    }

    public void DurateCheck()
    {
        if (mTimer.IsOver())
        {
            gameObject.SetActive(false);
        }
        mTimer.Update();
    }
}
