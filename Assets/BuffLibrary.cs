using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffLibrary : Singleton<BuffLibrary>
{
    public const float HEAL = 15f;
    public const float SPEEDUP = 0.2f;
    public const float POWER_BOOST = 0.3f;

    public float DeltaTime => Time.deltaTime * Time.timeScale;

    private IEnumerable HealBuff(uint level, AbilityTable ability)
    {
        ability.CurHealth += level * HEAL;

        yield break;
    }
    #region READ
    /// <summary>
    /// 즉발 : 체력을 회복하는 버프입니다. 
    /// <para>레벨 1당 HEAL만큼의 체력을 회복합니다.</para>
    /// </summary>
    #endregion
    public  IEnumerator Heal    (uint level, AbilityTable ability)
    {
        return HealBuff(level, ability).GetEnumerator();
    }

    private IEnumerable SpeedUpBuff(float durate, uint level, AbilityTable ability)
    {
        ability.IMoveSpeed += ability.MoveSpeed * level * SPEEDUP;

        for (float i = 0; i < durate; i += DeltaTime) { yield return null; }

        ability.IMoveSpeed -= ability.MoveSpeed * level * SPEEDUP;
    }
    #region READ
    /// <summary>
    /// 지속 : 이동속도를 증가시키는 버프입니다.
    /// <para>레벨 1당 SPEEDUP만큼의 이동속도 증가치를 반환합니다.</para>
    /// </summary>
    #endregion
    public  IEnumerator SpeedUp    (float durate, uint level, AbilityTable ability)
    {
        return SpeedUpBuff(durate, level, ability).GetEnumerator();
    }

    private IEnumerable PowerBoostBuff(float durate, uint level, AbilityTable ability)
    {
        ability.IAttackPower += ability.AttackPower * level * POWER_BOOST;

        for (float i = 0; i < durate; i += DeltaTime) { yield return null; }

        ability.IAttackPower -= ability.AttackPower * level * POWER_BOOST;
    }
    #region READ
    /// <summary>
    /// 지속 : 공격력을 증가시키는 버프입니다.
    /// </summary>
    #endregion
    public IEnumerator PowerBoost(float durate, uint level, AbilityTable ability)
    {
        yield return PowerBoostBuff(durate, level, ability);
    }
}
