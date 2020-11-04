using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinChief : MonoBehaviour, IObject, ICombatable, IAnimEventReceiver
{
    [Header("Ability")]
    [SerializeField] private AbilityTable AbilityTable;

    [Header("Skill Info")]
    [SerializeField] private SpecialTotem BuffTotem;
    [SerializeField] private SpecialTotem BombTotem;
    [SerializeField] private SpecialTotem LightningTotem;
    [SerializeField] private Lightning    Lightning;

    private Player mPlayer;

    public AbilityTable GetAbility => throw new System.NotImplementedException();

    public void AnimationPlayOver(AnimState anim)
    {
        
    }

    public void Damaged(float damage, GameObject attacker)
    {
        
    }

    public void IInit()
    {
        LightningTotem.SetSkill(() =>
        {
            Lightning.gameObject.SetActive(true);

            Lightning.SetAttackPower(15f);
            Lightning.transform.position += mPlayer.transform.position + (Vector3.up * 5f);
        });
    }

    public void IUpdate()
    {
        
    }

    public void PlayerEnter(MESSAGE message, Player enterPlayer)
    {
        mPlayer = enterPlayer;
    }

    public void PlayerExit(MESSAGE message)
    {
        if (message.Equals(MESSAGE.BELONG_FLOOR)) {
            mPlayer = null;
        }
    }

    public void CastBuff(BUFF buffType, IEnumerator castedBuff) {
        StartCoroutine(castedBuff);
    }

    public bool IsActive() {
        return gameObject.activeSelf;
    }

    public GameObject ThisObject() {
        return gameObject;
    }
}
