using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    [SerializeField] private Animator Animator;
    [SerializeField] private SpriteRenderer Renderer;
   
    private bool mHasPlayer;
    private Item mContainItem;

    public void Init(Item containItem)
    {
                          mContainItem = containItem;
        Renderer.sprite = mContainItem?.Sprite;

        Finger.Instance.Gauge.DisChargeEvent += Catch;
    }

    public void Catch()
    {
        if (mHasPlayer)
        {
            int animControlKey = Animator.GetParameter(0).nameHash;

            if (!Animator.GetBool(animControlKey))
            {
                Animator.SetBool(animControlKey, true);
            }
        }
    }

    private void AnimationPlayOver()
    {
        gameObject.SetActive(false);

        Inventory.Instance.AddItem(mContainItem);

        Finger.Instance.Gauge.DisChargeEvent -= Catch;
    }

    private void Reset()
    {
        Debug.Assert(TryGetComponent(out Animator));
        Debug.Assert(TryGetComponent(out Renderer));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) mHasPlayer = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) mHasPlayer = false;
    }
}
