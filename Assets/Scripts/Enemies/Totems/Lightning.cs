using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning : MonoBehaviour
{
    [SerializeField] private float mDamage;
    [SerializeField] private float mDurate;

    [SerializeField] private string[] mTargetTags;

    private Timer mTimer;

    public void SetDamage(float damage) => mDamage = damage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        for (int i = 0; i < mTargetTags.Length; ++i)
        {
            if (collision.CompareTag(mTargetTags[i]))
            {
                if (collision.TryGetComponent(out ICombatable combat)) combat.Damaged(mDamage, gameObject);
            }
        }
    }

    private void OnEnable()
    {
        (mTimer = mTimer ?? new Timer()).Start(mDurate);
    }

    public bool CanDisable()
    {
            mTimer.Update();
        if (mTimer.IsOver())
        {
            gameObject.SetActive(false);
            return true;
        }
        return false;
    }
}
