using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuicideTotem : MonoBehaviour, IObject, ICombatable
{
    [SerializeField] private StatTable mStat;

    private Dictionary<STAT_ON_TABLE, float> mStatTable;

    public StatTable Stat
    { get => mStat; }

    public void CastBuff(BUFF buffType, IEnumerator castedBuff) => StartCoroutine(castedBuff);

    public void Damaged(float damage, GameObject attacker)
    {
        if ((mStatTable[STAT_ON_TABLE.CURHEALTH] -= damage) <= 0) {
            gameObject.SetActive(false);
        }
    }

    public void IInit()
    {
        Debug.Assert(mStat.GetTable(gameObject.GetHashCode(), out mStatTable));
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
        
    }

    public void PlayerExit(MESSAGE message)
    {
        
    }

    public GameObject ThisObject() => gameObject;
}
