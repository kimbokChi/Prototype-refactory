using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BUFF
{
    HEAL, SPEEDUP, POWER_BOOST
}
public class BuffLibrary : Singleton<BuffLibrary>
{
    public const float HEAL = 15f;
    public const float SPEEDUP = 0.2f;
    public const float POWER_BOOST = 0.3f;

    public float DeltaTime => Time.deltaTime * Time.timeScale;

    private IEnumerable HealBuff(uint level, AbilityTable ability)
    {
        ability.Table[Ability.CurHealth] += level * HEAL;

        yield break;
    }
   
    private IEnumerable SpeedUpBuff(float durate, uint level, AbilityTable ability)
    {
        float increment = ability.Table[Ability.MoveSpeed] * level * SPEEDUP;

        ability.Table[Ability.IMoveSpeed] += increment;
        for (float i = 0; i < durate; i += DeltaTime) { yield return null; }
        ability.Table[Ability.IMoveSpeed] -= increment;
    }
  
    private IEnumerable PowerBoostBuff(float durate, uint level, AbilityTable ability)
    {
        float increment = ability.Table[Ability.AttackPower] * level * SPEEDUP;

        ability.Table[Ability.IAttackPower] += increment;
        for (float i = 0; i < durate; i += DeltaTime) { yield return null; }
        ability.Table[Ability.IAttackPower] -= increment;
    }
    
    #region READ
    /// <summary>
    /// 즉발 : 지속시간 없이, 시전된 즉시 효과를 발휘하는 버프를 반환합니다.
    /// </summary>
    #endregion
    public IEnumerator GetBurstBUFF(BUFF buff,uint level, AbilityTable ability)
    {
        switch (buff)
        {
            case BUFF.HEAL:
                return HealBuff(level, ability).GetEnumerator();

            default:
                Debug.Log($"The {buff} is not burst BUFF");
                return null;
        }
    }
    #region READ
    /// <summary>
    /// 지속 : 지속시간동안 효과를 발휘하는 버프를 반환합니다.
    /// </summary>
    #endregion
    public IEnumerator GetSlowBUFF(BUFF buff, uint level, float durateTime, AbilityTable ability)
    {
        switch (buff)
        {
            case BUFF.POWER_BOOST:
                return PowerBoostBuff(durateTime, level, ability).GetEnumerator();

            case BUFF.SPEEDUP:
                return SpeedUpBuff(durateTime, level, ability).GetEnumerator();

            default:
                Debug.Log($"The {buff} is not slow BUFF");
                return null;
        }
    }
}
