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
        ICombatable[] combats = mSenseArae.GetEnterTypeT<ICombatable>();

        for (int i = 0; i < combats.Length; ++i)
        {
            AbilityTable stat = combats[i].GetAbility;

            switch (mCastBuff)
            {
                case BUFF.HEAL:
                    combats[i].CastBuff(mCastBuff, BuffLibrary.Instnace.GetBurstBUFF(BUFF.HEAL, mLevel, stat));
                    break;

                case BUFF.SPEEDUP:
                    combats[i].CastBuff(mCastBuff, BuffLibrary.Instnace.GetSlowBUFF(BUFF.SPEEDUP, mLevel, mDurate, stat));
                    break;

                case BUFF.POWER_BOOST:
                    combats[i].CastBuff(mCastBuff, BuffLibrary.Instnace.GetSlowBUFF(BUFF.POWER_BOOST, mLevel,mDurate, stat));
                    break;
            }
        }
    }

    public void PlayerEnter(MESSAGE message, Player enterPlayer) { }
    public void PlayerExit (MESSAGE message) { }

    public GameObject ThisObject() => gameObject;
}
