using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffLibrary : Singleton<BuffLibrary>
{
    public const float DEFAULT_HEAL = 15f;

    public float DeltaTime => Time.deltaTime * Time.timeScale;

    private IEnumerable<float> HealBuff(uint level)
    {
        yield return DEFAULT_HEAL * level;
    }
    #region READ
    /// <summary>
    /// 즉발 : 체력을 회복하는 버프입니다. 
    /// <para>레벨 1당 상수DEFAULT_HEAL 만큼의 체력을 회복합니다.</para>
    /// </summary>
    #endregion
    public IEnumerator<float> Heal(uint level)
    {
        return HealBuff(level).GetEnumerator();
    }
}
