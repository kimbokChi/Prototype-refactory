using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuicideTotem : MonoBehaviour, IObject, ICombatable
{
    [SerializeField] private Area mArea;
    [SerializeField] private AbilityTable AbilityTable;
    [SerializeField] private float mFuseTime;

    private bool mIsOnFuse;

    public AbilityTable GetAbility => AbilityTable;

    public void CastBuff(BUFF buffType, IEnumerator castedBuff) => StartCoroutine(castedBuff);

    public void Damaged(float damage, GameObject attacker)
    {
        if ((AbilityTable.Table[Ability.CurHealth] -= damage) <= 0) {
            gameObject.SetActive(false);
        }
    }

    public void IInit()
    {
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
            combats[i].Damaged(AbilityTable.AttackPower, gameObject);
        }
        gameObject.SetActive(false);
    }

    public GameObject ThisObject() => gameObject;
}
