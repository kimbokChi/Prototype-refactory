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

    public void IInit()
    {
        mWaitForLighting = new Timer();

        mWaitForLighting.Start(mWaitLighting);
    }

    public bool IsActive()
    {
        return gameObject.activeSelf;
    }

    public void IUpdate()
    {
        if (mPlayer != null)
        {
            if (mWaitForLighting.IsOver())
            {
                if (mPlayer.Position(out Vector2 playerPos))
                {
                    // LIGHTING-!
                    Instantiate(mLighting, playerPos + mLightingOffset, Quaternion.identity);

                    mWaitForLighting.Start(mWaitLighting);
                }
            }
            else
            {
                mWaitForLighting.Update();
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

    public GameObject ThisObject() => gameObject;
}
