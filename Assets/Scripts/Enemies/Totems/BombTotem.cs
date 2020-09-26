using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombTotem : MonoBehaviour, IObject, ICombatable
{
    [SerializeField] private AbilityTable AbilityTable;

    [SerializeField] private float mTriggerRadius;

    private IEnumerator mEOnFuse;

    private Timer mWaitForFuse;

    private Player mPlayer;

    public AbilityTable GetAbility => AbilityTable;

    public void IInit()
    {
        mWaitForFuse = new Timer();

        mWaitForFuse.Start(0f);
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
                if (mWaitForFuse.IsOver())
                {
                    StartCoroutine(mEOnFuse = EOnFuse());
                }          
            }
            if (!mWaitForFuse.IsOver())
            {
                mWaitForFuse.Update();
            }
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

    private IEnumerator EOnFuse()
    {
        for (float i = 0; i < AbilityTable.BeginAttackDelay; i += Time.deltaTime * Time.timeScale) { yield return null; }

        if (OnTriggerPlayer())
        {
            mPlayer.Damaged(AbilityTable.AttackPower, gameObject);

        }
        mWaitForFuse.Start(AbilityTable.BeginAttackDelay);

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

    public void Damaged(float damage, GameObject attacker)
    {
        if ((AbilityTable.Table[Ability.CurHealth] -= damage) <= 0f)
        {
            gameObject.SetActive(false);
        }
    }

    public void CastBuff(BUFF buffType, IEnumerator castedBuff)
    {
        StartCoroutine(castedBuff);
    }
}
