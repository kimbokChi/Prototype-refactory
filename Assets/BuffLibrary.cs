using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffLibrary : Singleton<BuffLibrary>
{
    public const float HEAL = 15f;
    public const float SPEEDUP = 0.2f;

    public float DeltaTime => Time.deltaTime * Time.timeScale;

    private IEnumerable<float> HealBuff(uint level)
    {
        yield return HEAL * level;
    }
    #region READ
    /// <summary>
    /// 즉발 : 체력을 회복하는 버프입니다. 
    /// <para>레벨 1당 HEAL만큼의 체력을 회복합니다.</para>
    /// </summary>
    #endregion
    public  IEnumerator<float> Heal    (uint level)
    {
        return HealBuff(level).GetEnumerator();
    }

    private IEnumerable<float> SpeedUpBuff(float durate, uint level, float originSpeed)
    {
        for (float i = 0; i < durate; i += DeltaTime)
        {
            yield return originSpeed * level * SPEEDUP;
        }
    }
    #region READ
    /// <summary>
    /// 지속 : 이동속도를 증가시키는 버프입니다.
    /// <para>레벨 1당 SPEEDUP만큼의 이동속도 증가치를 반환합니다.</para>
    /// </summary>
    #endregion
    public  IEnumerator<float> SpeedUp    (float durate, uint level, float originSpeed)
    {
        return SpeedUpBuff(durate, level, originSpeed).GetEnumerator();
    }
}
