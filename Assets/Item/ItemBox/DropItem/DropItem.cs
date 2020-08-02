using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    private IEnumerator mEPlayAnimation;

    public bool IsCatch => (mEPlayAnimation != null);

    public void Catch()
    {
        if (TryGetComponent(out Animator animator))
        {
            animator.SetBool(animator.GetParameter(0).nameHash, true);

            StartCoroutine(mEPlayAnimation = EPlayAnimation());
        }
    }

    private IEnumerator EPlayAnimation()
    {
        for(float i = 0; i < 0.417f; i += Time.deltaTime * Time.timeScale)
        {
            yield return null;
        }
        gameObject.SetActive(false);

        yield break;
    }
}
