using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombTotem : MonoBehaviour, IObject
{
    [SerializeField] private float mTriggerRadius;

    [SerializeField] private float mWaitForBoom;
    [SerializeField] private float mDamage;

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

            if (OnTriggerPlayer())
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

        if (OnTriggerPlayer())
        {
            mPlayer.Damaged(mDamage, gameObject, out GameObject v);
        }
        mEOnFuse = null;
    }

    private bool OnTriggerPlayer()
    {
        if (mPlayer != null)
        {
            Vector2 playerPos = mPlayer.transform.position;

            return Vector2.Distance(playerPos, transform.position) <= mTriggerRadius;
        }
        return false;
    }

    public GameObject ThisObject() => gameObject;
}
