using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : MonoBehaviour
{
    public const string TRIGGER_TAG = "Player";

    [SerializeField] private GameObject ShowItem;
    [SerializeField] private Animator   Animator;

    private bool mIsPlayerEnter;

    private Item mContainItem;
    private DropItem mDropItem;

    private void OnEnable()
    {
        if (ShowItem.transform.GetChild(0).TryGetComponent(out mDropItem))
        {
            if (mDropItem.gameObject.TryGetComponent(out SpriteRenderer render))
            {
                 mContainItem = ItemLibrary.Instance.GetRandomItem();
                render.sprite = mContainItem?.Sprite;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(TRIGGER_TAG) && !ShowItem.activeSelf) {
            mIsPlayerEnter   = true;
            Animator.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(TRIGGER_TAG)) {
            mIsPlayerEnter = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (mIsPlayerEnter && !mDropItem.IsCatch)
            {
                mDropItem.Catch();
                Inventory.Instance.AddItem(mContainItem);
            }
        }
    }
}
