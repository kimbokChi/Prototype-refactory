using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossTotem : MonoBehaviour, IObject, ICombatable
{
    public enum SHOOTING_TYPE
    {
        CROSS, XSHAPE
    }

    [SerializeField] private SHOOTING_TYPE mShootingType;

    [SerializeField] private StatTable mStat;

    [SerializeField] private Arrow mDartOrigin;

    [SerializeField] private float mDartSpeed;
    [SerializeField] private float mWaitNextShoot;

    private Dictionary<STAT_ON_TABLE, float> mStatTable;

    private Pool<Arrow> mDartPool;

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
        mDartPool = new Pool<Arrow>();

        mDartPool.Init(mDartOrigin, Pool_popMethod, Pool_addMethod, Pool_returnToPool);

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
        mDartPool.Update();

        if (mWaitForShoot.IsOver())
        {
            for (int i = 0; i < 4; i++)
            {
                Arrow arrow = mDartPool.Pop();

                switch (mShootingType)
                {
                    case SHOOTING_TYPE.CROSS:
                        switch (i)
                        {
                            case 0:
                                arrow.Setting(mDartSpeed, Vector2.left);
                                break;

                            case 1:
                                arrow.Setting(mDartSpeed, Vector2.right);
                                break;

                            case 2:
                                arrow.Setting(mDartSpeed, Vector2.down);
                                break;

                            case 3:
                                arrow.Setting(mDartSpeed, Vector2.up);
                                break;
                            default:
                                break;
                        }
                        break;

                    case SHOOTING_TYPE.XSHAPE:
                        float rotation = (45f + 90f * i) * Mathf.Deg2Rad;

                        arrow.Setting(mDartSpeed, new Vector2(Mathf.Cos(rotation), Mathf.Sin(rotation)));
                        break;
                }
                arrow.transform.position = transform.position;

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

    private void Arrow_targetHit(ICombatable combat)
    {
        combat.Damaged(mStat.RAttackPower, gameObject, out GameObject v);
    }

    private bool Arrow_canDistroy(uint hitCount)
    {
        if (hitCount > 0) return true;

        return false;
    }

    private void Pool_popMethod(Arrow arrow)
    {
        arrow.transform.position = transform.position;

        arrow.gameObject.SetActive(true);
    }
    private void Pool_addMethod(Arrow arrow)
    {
        arrow.gameObject.SetActive(false);
    }
    private bool Pool_returnToPool(Arrow arrow)
    {
        return Vector2.Distance(transform.position, arrow.transform.position) > 7f || !arrow.gameObject.activeSelf;
    }
}
