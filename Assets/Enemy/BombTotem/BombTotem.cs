using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombTotem : MonoBehaviour, IObject
{
    [SerializeField] private float mTriggerRadius;

    [SerializeField] private float mWaitForBoom;

    private IEnumerator mEOnFuse;

    private Player mPlayer;

    public void IInit()
    {
    }

    public bool IsActive()
    {
        return gameObject.activeSelf;
    }

    public void IUpdate()
    {
        if (mPlayer != null && mEOnFuse == null)
        {
            Vector2 playerPos = mPlayer.transform.position;

            if (Vector2.Distance(playerPos, transform.position) <= mTriggerRadius)
            {
                StartCoroutine(mEOnFuse = EOnFuse());
            }
        }
    }

    public void PlayerEnter(Player enterPlayer)
    {
        mPlayer = enterPlayer;
    }

    public void PlayerExit()
    {
        mPlayer = null;
    }

    private IEnumerator EOnFuse()
    {
        for (float i = 0; i < mWaitForBoom; i += Time.deltaTime * Time.timeScale) { yield return null; }

        mEOnFuse = null;
    }

    public GameObject ThisObject() => gameObject;
}
