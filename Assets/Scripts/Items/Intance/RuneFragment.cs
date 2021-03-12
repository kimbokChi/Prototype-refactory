using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneFragment : Item
{
    private readonly Vector3 EquipPosition = new Vector3(0.03f, 0.15f, 0f);

    [Header("RuneFragment Property")]
    [SerializeField] private Animator _Animator;
    private int _AnimatorHash;

    [SerializeField] private Area _AttackArea;
    [SerializeField, Range(0f, 1f)] private float _NeedCharge;

    private float _ChargingPower = 0f;
    private bool _IsAlreadyInit = false;
    private GameObject _Player;

    private enum Anim
    {
        Default, Attack, Charging_Begin, Charging_End
    }
    public override void AttackAction(GameObject attacker, ICombatable combatable)
    {
        _Animator.SetInteger(_AnimatorHash, (int)Anim.Attack);
    }
    protected override void AttackAnimationPlayOver()
    {
        base.AttackAnimationPlayOver();

        _Animator.SetInteger(_AnimatorHash, (int)Anim.Default);
    }
    public override void AttackCancel()
    {
        _Animator.SetInteger(_AnimatorHash, (int)Anim.Default);
    }
    public override void OffEquipThis(SlotType offSlot)
    {
        
    }
    public override void OnEquipThis(SlotType onSlot)
    {
        Init();

        switch (onSlot)
        {
            case SlotType.Accessory:
                break;

            case SlotType.Weapon:
                transform.localPosition = EquipPosition;

                if (_Player == null) {
                    _Player = transform.parent.parent.gameObject;
                }
                Inventory.Instance.ChargeBeginAction += ChargeBeginAction;
                Inventory.Instance.ChargeEndAction   += ChargeEndAction;
                break;
        }
    }

    private void ChargeBeginAction()
    {
        _Animator.SetInteger(_AnimatorHash, (int)Anim.Charging_Begin);
    }
    private void ChargeEndAction(float charge)
    {
        if (charge >= _NeedCharge)
        {
            _ChargingPower = charge;
            _Animator.SetInteger(_AnimatorHash, (int)Anim.Charging_End);
        }
        else 
            _Animator.SetInteger(_AnimatorHash, (int)Anim.Default);
    }

    private void Init()
    {
        if (!_IsAlreadyInit)
        {
            _IsAlreadyInit = true;
            _AnimatorHash  = _Animator.GetParameter(0).nameHash;

            _AttackArea.SetEnterAction(o => 
            {
                if (o.TryGetComponent(out ICombatable combatable))
                {
                    combatable.Damaged(StatTable[ItemStat.AttackPower], _Player);
                    Inventory.Instance.OnAttackEvent(_Player, combatable);
                }
            });
        }
    }
    private void ChargingSkill()
    {
        MainCamera.Instance.Shake(0.5f, 1f);
        // To do ...
        _ChargingPower = 0f;
    }
}
