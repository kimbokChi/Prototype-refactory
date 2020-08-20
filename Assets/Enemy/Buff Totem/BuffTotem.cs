using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffTotem : MonoBehaviour, IObject
{
    [SerializeField] private float mWaitCastTime;

    [SerializeField] private Area mSenseArae;

    [SerializeField] private BUFF  mCastBuff;
    [SerializeField] private float mDurate;
    [SerializeField] private uint  mLevel;

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
            StatTable stat = combats[i].Stat;

            switch (mCastBuff)
            {
                case BUFF.HEAL:
                    combats[i].CastBuff(BUFF.HEAL, BuffLibrary.Instnace.Heal(mLevel, stat));
                    break;

                case BUFF.SPEEDUP:
                    combats[i].CastBuff(BUFF.SPEEDUP, BuffLibrary.Instnace.SpeedUp(mDurate, mLevel, stat));
                    break;
            }
        }
    }

    public void PlayerEnter(Player enterPlayer) { }
    public void PlayerExit() { }

    public GameObject ThisObject() => gameObject;
}
