using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuicideTotem : MonoBehaviour, IObject, ICombatable
{
    [SerializeField] private Area mArea;
    [SerializeField] private AbilityTable mAbilityTable;
    [SerializeField] private float mFuseTime;

    private Dictionary<STAT_ON_TABLE, float> mPersonalTable;

    private bool mIsOnFuse;

    public AbilityTable GetAbility => mAbilityTable;

    public void CastBuff(BUFF buffType, IEnumerator castedBuff) => StartCoroutine(castedBuff);

    public void Damaged(float damage, GameObject attacker)
    {
        if ((mPersonalTable[STAT_ON_TABLE.CURHEALTH] -= damage) <= 0) {
            gameObject.SetActive(false);
        }
    }

    public void IInit()
    {
        Debug.Assert(mAbilityTable.GetTable(gameObject.GetHashCode(), out mPersonalTable));

        mIsOnFuse = false;
    }

    public bool IsActive()
    {
        return gameObject.activeSelf;
    }

    public void IUpdate()
    {
        
    }

    public void PlayerEnter(MESSAGE message, Player enterPlayer)
    {
        if (message.Equals(MESSAGE.THIS_ROOM) && !mIsOnFuse)
        {
            mIsOnFuse = true;

            StartCoroutine(EOnFuse());
        }
    }

    public void PlayerExit(MESSAGE message)
    {
        
    }

    private IEnumerator EOnFuse()
    {
        for (float i = 0; i < mFuseTime; i += Time.deltaTime * Time.timeScale) { yield return null; }

        ICombatable[] combats = mArea.GetEnterTypeT<ICombatable>();

        for (int i = 0; i < combats.Length; ++i)
        {
            combats[i].Damaged(mAbilityTable.RAttackPower, gameObject);
        }
        gameObject.SetActive(false);
    }

    public GameObject ThisObject() => gameObject;
}
