using System;
using System.Collections;
using UnityEngine;

public class TracerBullet : MonoBehaviour
{
    [SerializeField] private float LifeTime;
    [SerializeField] private float Speed;

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
        float DeltaTime()
        {
            return Time.deltaTime * Time.timeScale;
        }
        for (float i = 0f; i < LifeTime; i += DeltaTime())
        {
            Vector3 direction
                = TargetTransform.position - transform.position;

            transform.position += direction.normalized * Speed;

            yield return null;
        }
        ETarceTarget = null;
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
}
