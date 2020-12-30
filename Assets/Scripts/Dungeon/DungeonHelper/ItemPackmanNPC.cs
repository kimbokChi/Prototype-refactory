using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kimbokchi;

public class ItemPackmanNPC : MonoBehaviour
{
    [SerializeField] private GameObject InteractionButton;

    [SerializeField] private DropItem DropItem;
    [SerializeField] private Transform DropItemHolder;

    [Header("ItemDrop Curve")]
    [SerializeField] private float CurveSpeed; [Space()]
    [SerializeField] private Vector2 PointA;
    [SerializeField] private Vector2 PointB;
    [SerializeField] private Vector2 PointC;
    [SerializeField] private Vector2 PointD;

    private bool mHasPlayer;
    private bool mIsGivingItem;

    public void Interaction()
    {
        if (!mIsGivingItem)
        {
            DropItem.Init(ItemLibrary.Instance.GetRandomItem());
            DropItem.gameObject.SetActive(true);

            StartCoroutine(ItemDrop());

            mIsGivingItem = true;
            EffectLibrary.Instance.UsingEffect(EffectKind.Twinkle, transform.position);
        }
        else
        {
            SystemMessage.Instance.ShowMessage("이미 한번 아이템이\n지급되었습니다!");
        }
    }

    private IEnumerator ItemDrop()
    {
        for (float ratio = 0f; ratio < 1; ratio += Time.deltaTime * CurveSpeed)
        {
            ratio = ratio > 1 ? 1 : ratio;

            DropItemHolder.localPosition = Utility.BezierCurve3(PointA, PointB, PointC, PointD, ratio);
            yield return null;
        }
        yield return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) InteractionButton.SetActive(mHasPlayer = true);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) InteractionButton.SetActive(mHasPlayer = false);
    }
}
