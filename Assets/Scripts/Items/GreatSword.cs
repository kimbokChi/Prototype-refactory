using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreatSword : Item
{
    [SerializeField] private Animator Animator;
    [SerializeField] private Area AttackArea;

    [Header("Charge Ablity")]
    [SerializeField] private Projection SwordDance;
    [Range(0f, 1f)]
    [SerializeField] private float DemandCharge;

    private int mAnimControlKey;
    private int mAnimPlayKey;

    private GameObject mPlayer;

    public override void AttackAction(GameObject attacker, ICombatable combatable)
    {
        base.AttackAction(attacker, combatable);
    }

    public override void OffEquipThis(SlotType offSlot)
    {
        switch (offSlot)
        {
            case SlotType.Weapon:
                Inventory.Instance.ChargeAction -= ChargeAction;
                break;
        }
    }

    public override void OnEquipThis(SlotType onSlot)
    {
        switch (onSlot)
        {
            case SlotType.Weapon:
                Inventory.Instance.ChargeAction += ChargeAction;
                break;
        }
    }

    private void ChargeAction(float charge)
    {

    }

    protected override void CameraShake()
    {
        
    }
}
