using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour
{
    private Collider2D mChallenger;

    private CircleCollider2D mCircle;



    public Collider2D GetChallenger()
    {
        return mChallenger;
    }
    public void SetRange(float radius)
    {
        if (mCircle) mCircle.radius = radius;
    }

    private void Start() => TryGetComponent(out mCircle);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (mChallenger == null && collision.CompareTag("Enemy"))
        {
            mChallenger = collision;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (mChallenger == collision) mChallenger = null;
    }
}
