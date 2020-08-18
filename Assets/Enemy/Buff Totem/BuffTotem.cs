﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffTotem : MonoBehaviour, IObject
{
    [SerializeField] private float mWaitCastTime;

    [SerializeField] private Area mSenseArae;

    private Timer mWaitForCastBuff;

    public void IInit()
    {
        mWaitForCastBuff = new Timer();

        mWaitForCastBuff.Start(mWaitCastTime);
    }

    public bool IsActive()
    {
        return gameObject.activeSelf;
    }

    public void IUpdate()
    {
        if (mWaitForCastBuff.IsOver())
        {
            CastBuff();

            mWaitForCastBuff.Start(mWaitCastTime);
        }
        else
        {
            mWaitForCastBuff.Update();
        }
    }

    private void CastBuff()
    {
        ICombat[] combats = mSenseArae.GetEnterTypeT<ICombat>();

        for (int i = 0; i < combats.Length; ++i)
        {
            combats[i].Damaged(-20f, gameObject, out GameObject v);
        }
    }

    public void PlayerEnter(Player enterPlayer) { }
    public void PlayerExit() { }

    public GameObject ThisObject() => gameObject;
}
