using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingTotem : MonoBehaviour, IObject
{
    [SerializeField] private Lighting mLighting;

    [SerializeField] private float mWaitLighting;

    [SerializeField] private Vector2 mLightingOffset;

    private Player mPlayer;

    private Timer mWaitForLighting;

    private Pool<Lighting> mPool;

    public void IInit()
    {
        mWaitForLighting = new Timer();

        mWaitForLighting.Start(mWaitLighting);

        mPool = new Pool<Lighting>();
        mPool.Init(mLighting, Pool_popMethod, Pool_addMethod, Pool_returnToPool);
    }

    public bool IsActive()
    {
        return gameObject.activeSelf;
    }

    public void IUpdate()
    {
        mPool.Update();

        if (mPlayer != null)
        {
            if (mWaitForLighting.IsOver())
            {
                if (mPlayer.Position(out Vector2 playerPos))
                {
                    mPool.Pop();

                    mWaitForLighting.Start(mWaitLighting);
                }
            }
            else
            {
                mWaitForLighting.Update();
            }
        }
    }

    public void PlayerEnter(MESSAGE message, Player enterPlayer)
    {
        mPlayer = enterPlayer;
    }

    public void PlayerExit(MESSAGE message)
    {
        if (message.Equals(MESSAGE.BELONG_FLOOR))
        {
            mPlayer = null;
        }
    }

    public GameObject ThisObject() => gameObject;

    private void Pool_popMethod(Lighting lighting)
    {
        mPlayer.Position(out Vector2 playerPos);

        lighting.transform.position = mLightingOffset + playerPos;

        lighting.gameObject.SetActive(true);
    }
    private void Pool_addMethod(Lighting lighting)
    {
        lighting.gameObject.SetActive(false);
    }
    private bool Pool_returnToPool(Lighting lighting)
    {
        lighting.DurateCheck();

        return !lighting.gameObject.activeSelf;
    }
}
