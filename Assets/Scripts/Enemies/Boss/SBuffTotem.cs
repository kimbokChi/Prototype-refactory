using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBuffTotem : MonoBehaviour
{
    public Action<SBuffTotem> CastOverAction;

    [SerializeField] private Area BuffArea;

    [Header("Buff Info")]
    [SerializeField] private  uint BuffLevel;
    [SerializeField] private float BuffDurate;

    [Header("Summon Enemy Info")]
    [SerializeField] private GameObject[] SummonEnemies;

    private Room mSummonRoom;

    public void Init()
    {
        BuffArea.SetEnterAction(o => {

            if (o.TryGetComponent(out ICombatable combatable))
            {
                var buffLib = BuffLibrary.Instance;
                var ability = combatable.GetAbility;

                combatable.CastBuff(BUFF.POWER_BOOST, 
                    buffLib.GetSlowBUFF(BUFF.POWER_BOOST, BuffLevel, BuffDurate, ability));

                combatable.CastBuff(BUFF.SPEEDUP,
                    buffLib.GetSlowBUFF(BUFF.SPEEDUP, BuffLevel, BuffDurate, ability));
            }
        });
    }
    public void Cast(Room summonRoom, Vector2 castPoint)
    {
        mSummonRoom = summonRoom;
        transform.position = castPoint;

        gameObject.SetActive(true);
    }

    // 애니메이션 이벤트로 실행될 함수
    private void SummonEnemy()
    {
        for (int i = -1; i < 2; i++)
        {
            int index = UnityEngine.Random.Range(0, SummonEnemies.Length);
            var enemy = Instantiate(SummonEnemies[index], mSummonRoom.transform);

            if (enemy.TryGetComponent(out IObject iobject))
            {
                mSummonRoom.AddIObject(iobject);
            }
            enemy.transform.localPosition += Vector3.right * transform.position.x;
            enemy.transform.localPosition += Vector3.left  * i;
        }
    }
    // 애니메이션 이벤트로 실행될 함수
    // (애니메이션이 끝나는 타이밍에)
    private void AnimationPlayOver()
    {
        gameObject.SetActive(false);

        CastOverAction?.Invoke(this);
    }
}
