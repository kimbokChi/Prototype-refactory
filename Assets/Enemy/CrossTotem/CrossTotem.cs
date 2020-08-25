using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossTotem : MonoBehaviour, IObject, ICombat
{
    [SerializeField] private StatTable mStat;

    [SerializeField] private Arrow mDartOrigin;

    [SerializeField] private float mDartSpeed;
    [SerializeField] private float mWaitNextShoot;

    private Dictionary<STAT_ON_TABLE, float> mStatTable;

    private Player mPlayer;

    private Timer mWaitForShoot;

    public StatTable Stat => mStat;

    public void CastBuff(BUFF buffType, IEnumerator castedBuff)
    {
        StartCoroutine(castedBuff);
    }

    public void Damaged(float damage, GameObject attacker, out GameObject victim)
    {
        victim = gameObject;

        if ((mStatTable[STAT_ON_TABLE.CURHEALTH] -= damage) <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public void IInit()
    {
        Debug.Assert(mStat.GetTable(gameObject.GetHashCode(), out mStatTable));

        mWaitForShoot = new Timer();

        mWaitForShoot.Start(mWaitNextShoot);
    }

    public bool IsActive()
    {
        return gameObject.activeSelf;
    }

    public void IUpdate()
    {
        if (mWaitForShoot.IsOver())
        {
            for (int i = 0; i < 4; i++)
            {
                Arrow arrow = Instantiate(mDartOrigin, transform.position, Quaternion.identity);

                arrow.Setting(mDartSpeed, new Vector2(Mathf.Cos((45f + 90f * i) * Mathf.Deg2Rad), Mathf.Sin((45f + 90f * i) * Mathf.Deg2Rad)));

                arrow.Setting(Arrow_targetHit, Arrow_canDistroy);
            }
            mWaitForShoot.Start(mWaitNextShoot);
        }
        else
        {
            mWaitForShoot.Update();
        }
    }

    public void PlayerEnter(MESSAGE message, Player enterPlayer)
    {
        if (message.Equals(MESSAGE.THIS_ROOM))
        {
            mPlayer = enterPlayer;
        }
    }

    public void PlayerExit(MESSAGE message)
    {
        mPlayer = null;
    }

    public GameObject ThisObject() => gameObject;

    private void Arrow_targetHit(ICombat combat)
    {
        combat.Damaged(mStat.RAttackPower, gameObject, out GameObject v);
    }

    private bool Arrow_canDistroy(uint hitCount)
    {
        if (hitCount > 0) return true;

        return false;
    }
}
