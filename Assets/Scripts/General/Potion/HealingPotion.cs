using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HealingPotion : Potion
{
    [SerializeField, Range(0f, 1f)] private float _HealRate;

    public override void UsePotion()
    {
        float maxHealth = PotionPool.Instance.PlayerAbilityTable[Ability.MaxHealth];
        float heal = Mathf.Min(maxHealth, maxHealth * _HealRate);

        PotionPool.Instance.PlayerAbilityTable.Table[Ability.CurHealth] += heal;
        PotionPool.Instance.Add(this);
    }
    public override void Interaction()
    {
        if (_TriggerEntryPlayer) {

            UsePotion();
        }
    }
}
