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

    public float COMMON_PROBABILITY = 0.4f; // 40%
    public float RARE_PROBABILITY = 0.35f; // 35%
    public float EPIC_PROBABILITY = 0.2f; // 20%
    public float LEGENDARY_PROBABILITY = 0.05f; //  5%

    private float[] mProbabilityArray;

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
        mProbabilityArray = new float[4]
        {
            COMMON_PROBABILITY, RARE_PROBABILITY, EPIC_PROBABILITY, LEGENDARY_PROBABILITY
        };
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

    public void ReplaceItem()
    {
        float probability = Random.value;

        float sum = 0f;

        for (int i = 0; i < 4; i++)
        {
            sum += mProbabilityArray[i];

            if (sum <= probability)
            {
                if (ShowItem.transform.GetChild(0).TryGetComponent(out mDropItem))
                {
                    if (mDropItem.gameObject.TryGetComponent(out SpriteRenderer render))
                    {
                        mContainItem = ItemLibrary.Instance.GetRandomItem((ItemRating)i);
                        render.sprite = mContainItem?.Sprite;
                    }
                }
            }
        }
    }
}
