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

    private Player mPlayer;

    public AbilityTable GetAbility => AbilityTable;

    public void AnimationPlayOver(AnimState anim)
    {
        
    }

    public void Damaged(float damage, GameObject attacker)
    {
        
    }

    public void IInit()
    {
        BuffTotem.SetAreaEnterAction(o =>
        {
            if (o.TryGetComponent(out ICombatable combatable))
            {
                var ability = combatable.GetAbility;
                var buffLib = BuffLibrary.Instance;

                combatable.CastBuff(BUFF.POWER_BOOST, 
                    buffLib.GetSlowBUFF(BUFF.POWER_BOOST, 3, 5f, ability));

                combatable.CastBuff(BUFF.SPEEDUP, 
                    buffLib.GetSlowBUFF(BUFF.SPEEDUP, 3, 5f, ability));
            }
        });

        BombTotem.SetAreaEnterAction(o =>
        {
            if (o.TryGetComponent(out ICombatable combatable))
            {
                combatable.Damaged(20f, gameObject);
            }
        });
    }

    public void IUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            BuffTotem.CastSkill(Vector2.right * 2f);
        }
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
