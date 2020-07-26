using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    private IEnumerator mCRplayAnimation = null;

    public bool IsCatch => (mCRplayAnimation != null);

    public void Catch()
    {
        if (TryGetComponent(out Animator animator))
        {
            animator.SetBool(animator.GetParameter(0).nameHash, true);

            StartCoroutine(mCRplayAnimation = CR_playAnimation());
        }
    }

    private IEnumerator CR_playAnimation()
    {
        for(float i = 0; i < 0.417f; i += Time.deltaTime * Time.timeScale)
        {
            yield return null;
        }
        gameObject.SetActive(false);

        yield break;
    }
}
