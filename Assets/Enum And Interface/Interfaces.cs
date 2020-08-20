using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* IObject
 * 플레이어나 적같은 오브젝트들은 해당 인터페이스를 구현하여 초기화/업데이트를 할때 사용한다.
 * 
 * 이렇게 하는 이유는, 방안에 있는 오브젝트들의 초기화와 업데이트를 제어할 수 있도록 하기 위해서그렇다.
 * 
 * 사용방법 : 
 * - 플레이어가 층에 진입->층에 존재하는 모든 IObject의 IInit함수를 실행
 * - 플레이어가 존재하는 층에 모든 IObject의 IUpdate실행
 */
public interface IObject
{
    void IInit();
    void IUpdate();

    void PlayerEnter(Player enterPlayer);
    void PlayerExit();

    bool IsActive();

    GameObject ThisObject();
}
public interface ICombat
{
    StatTable Stat
    {
        get;
    }

    void Damaged(float damage, GameObject attacker, out GameObject victim);

    void CastBuff(BUFF buffType, IEnumerator castedBuff);
}