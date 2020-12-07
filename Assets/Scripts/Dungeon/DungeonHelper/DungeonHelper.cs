using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kimbokchi;

public class DungeonHelper : MonoBehaviour
{
    [SerializeField] private DropItem DropItem;
    [SerializeField] private Transform DropItemHolder;

    [Header("ItemDrop Curve")]
    [SerializeField] private float CurveSpeed; [Space()]
    [SerializeField] private Vector2 PointA;
    [SerializeField] private Vector2 PointB;
    [SerializeField] private Vector2 PointC;
    [SerializeField] private Vector2 PointD;

    private bool mHasPlayer;

    private void OnEnable()
    {
        DropItem.Init(ItemLibrary.Instance.GetRandomItem());

        Finger.Instance.Gauge.DisChargeEvent += Interaction;
    }

    private void Interaction()
    {
        if (mHasPlayer)
        {
            DropItem.gameObject.SetActive(true);

            StartCoroutine(ItemDrop());
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
        Finger.Instance.Gauge.DisChargeEvent -= Interaction;

        yield return null;
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
