using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : MonoBehaviour
{
    public const string TRIGGER_TAG = "Player";
    
    private Animator mAnimator;

    private bool mIsOpen;
    private bool mIsPlayerEnter;

    private Item mContainItem;
    private DropItem mDropItem;

    private void OnEnable()
    {
        mIsOpen = false;

        if (TryGetComponent(out mAnimator))
        {
            mAnimator.enabled = false;
        }

        mContainItem = ItemLibrary.Instance.GetRandomItem();

        if (transform.GetChild(0).TryGetComponent(out mDropItem))
        {
            if (mContainItem != null)
            {
                if (mDropItem.gameObject.TryGetComponent(out SpriteRenderer render))
                {
                    render.sprite = mContainItem.Sprite;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(TRIGGER_TAG) && !mIsOpen)
        {
            mAnimator.enabled = true;

            StartCoroutine(CR_openBox());

            mIsPlayerEnter = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(TRIGGER_TAG))
        {
            mIsPlayerEnter = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (mIsPlayerEnter && mIsOpen && !mDropItem.IsCatch)
            {
                mDropItem.Catch();

                Inventory.Instance.AddItem(mContainItem);
            }
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

            mDropItem.gameObject.SetActive(true);
        }
        yield break;
    }
}
