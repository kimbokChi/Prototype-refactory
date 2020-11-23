using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Buff
{
    Heal, SpeedUp, PowerBoost, Stun
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
    public IEnumerator Stun(float duration, AbilityTable ability)
    {
        return StunBuff(duration, ability).GetEnumerator();
    }
    private IEnumerable StunBuff(float duration, AbilityTable ability)
    {
        for (float i = 0; i < duration; i += DeltaTime) 
        {
            ability.Table[Ability.IMoveSpeed] = -ability.Table[Ability.MoveSpeed];
            yield return null;

            ability.Table[Ability.IMoveSpeed] = -ability.Table[Ability.MoveSpeed];
        }
        ability.Table[Ability.IMoveSpeed] = 0f;
    }

    public IEnumerator GetBuff(Buff buff, uint level, float duration, AbilityTable ability)
    {
        switch (buff)
        {
            case Buff.Heal:
                return HealBuff(level, ability).GetEnumerator();

            case Buff.SpeedUp:
                return SpeedUpBuff(duration, level, ability).GetEnumerator();

            case Buff.PowerBoost:
                return PowerBoostBuff(duration, level, ability).GetEnumerator();

            default:
                Debug.LogError("Undefined");
                return null;
        }
    }
}
