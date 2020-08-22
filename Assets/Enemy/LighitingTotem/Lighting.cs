using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lighting : MonoBehaviour
{
    [SerializeField] private float mDamage;

    [SerializeField] private string[] mTargetTags;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        for (int i = 0; i < mTargetTags.Length; ++i)
        {
            if (collision.CompareTag(mTargetTags[i]))
            {
                if (collision.TryGetComponent(out ICombat combat))
                {
                    combat.Damaged(mDamage, gameObject, out GameObject v);
                }
            }
        }
    }
}
