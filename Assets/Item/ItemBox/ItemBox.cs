using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : MonoBehaviour, IObject
{
    [SerializeField] private Sprite mOpenBox;
    
    private Animator mAnimator;

    private bool mIsOpen;

    public void IInit()
    {

    }

    public void IUpdate()
    {
        
    }

    private void Start()
    {
        mIsOpen = false;

        if (TryGetComponent(out mAnimator))
        {
            mAnimator.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !mIsOpen)
        {
            mAnimator.enabled = true;

            StartCoroutine(CR_openBox());
        }
    }

    private IEnumerator CR_openBox()
    {
        for (float i = 0; i < 0.333f; i += Time.deltaTime * Time.timeScale)
        {
            yield return null;
        }

        if (TryGetComponent(out SpriteRenderer renderer))
        {
            mIsOpen = true;

            renderer.sprite = mOpenBox;
        }
        yield break;
    }
}
