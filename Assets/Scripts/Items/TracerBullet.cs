using System;
using System.Collections;
using UnityEngine;
using Kimbokchi;

public class TracerBullet : MonoBehaviour
{
    public Action<TracerBullet> DisableAction;

    [SerializeField] private float LifeTime;

    private Transform TargetTransform;
    private IEnumerator ETarceTarget;
    private Action<GameObject> TargetHitAction;

    public bool Shoot(Transform target, Action<GameObject> hitAction)
    {
        bool canShoot = ETarceTarget == null;

        if (canShoot)
        {
            TargetTransform = target;
            TargetHitAction = hitAction;

            StartCoroutine(ETarceTarget = TarceTarget());
        }
        return canShoot;
    }

    private IEnumerator TarceTarget()
    {
        Vector2 pointA = transform.position;
        Vector2 pointB = transform.position;

        Vector3 offset = Vector2.left;

        if (TargetTransform.position.y < transform.position.y)
        {
            offset += Vector3.up * 1.5f;

            pointB += new Vector2(1.5f, -3);
        }
        else
        {
            offset -= Vector3.up * 1.5f;

            pointB += new Vector2(1.5f, +3);
        }

        float DeltaTime()
        {
            return Time.deltaTime * Time.timeScale;
        }
        for (float i = 0f; i < LifeTime; i += DeltaTime())
        {
            var targetPoint = TargetTransform.position;

            transform.BezierCurve3
                (pointA, pointB, targetPoint + offset, targetPoint, i / LifeTime);

            yield return null;
        }
        ETarceTarget = null;
        
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.Equals(TargetTransform))
        {
            TargetHitAction.Invoke(collision.gameObject);

            if (ETarceTarget != null)
            {
                StopCoroutine(ETarceTarget);

                ETarceTarget = null;
            }
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        DisableAction?.Invoke(this);
    }
}
