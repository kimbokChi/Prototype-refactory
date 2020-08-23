using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : EnemyBase, IObject, ICombat
{
    [SerializeField]
    private StatTable mStat;

    [SerializeField]
    private Arrow mArrow;

    
    private Dictionary<STAT_ON_TABLE, float> mStatTable;

    public override StatTable Stat => mStat;

    public override void Damaged(float damage, GameObject attacker, out GameObject victim)
    {
        victim = gameObject;

        if ((mStatTable[STAT_ON_TABLE.CURHEALTH] -= damage) <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public override void IInit()
    {
        Debug.Assert(mStat.GetTable(gameObject.GetHashCode(), out mStatTable));

        mArrow.Setting(Arrow_targetHit, Arrow_canDistroy);
    }

    public override void IUpdate()
    {
    }

    public override void PlayerEnter(MESSAGE message, Player enterPlayer)
    {
    }

    public override void PlayerExit(MESSAGE message)
    {
    }

    public override void CastBuff(BUFF buffType, IEnumerator castedBuff)
    {
        StartCoroutine(castedBuff);
    }
    public override bool IsActive()
    {
        return gameObject.activeSelf;
    }
    public override GameObject ThisObject() => gameObject;

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
