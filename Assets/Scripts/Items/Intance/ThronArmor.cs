using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThronArmor : Item
{
    [SerializeField] private Area _CollisionArea;
    [SerializeField] private Animator _Animator;
    [SerializeField, Range(0f, 1f)] private float _MirrorRate;

    private int _AnimatorHash;
    private bool _IsAlreadyInit = false;

    private Player _Player;

    public override void AttackCancel()
    {
        _Animator.SetBool(_AnimatorHash, false);
    }

    public override void OffEquipThis(SlotType offSlot)
    {
        switch (offSlot)
        {
            case SlotType.Accessory:
                Inventory.Instance.BeDamagedAction -= PlayerDamagedAction;
                break;

            case SlotType.Weapon:
                break;
        }
    }
    public override void OnEquipThis(SlotType onSlot)
    {
        Init();

        switch (onSlot)
        {
            case SlotType.Accessory:
                Inventory.Instance.BeDamagedAction += PlayerDamagedAction;
                break;

            case SlotType.Weapon:
                {
                    if (_Player == null)
                    {
                        transform.parent.parent.TryGetComponent(out _Player);

                        _CollisionArea.SetEnterAction(AreaEnterAction);
                    }
                }
                break;
        }
    }
    private void Init()
    {
        if (!_IsAlreadyInit)
        {
            _AnimatorHash = _Animator.GetParameter(0).nameHash;
        }
    }
    private void PlayerDamagedAction(ref float damage, GameObject attacker, GameObject victim)
    {
        if (attacker.TryGetComponent(out ICombatable combatable))
        {
            combatable.Damaged(damage * _MirrorRate, victim);
        }
    }
    private void AreaEnterAction(GameObject collider)
    {
        if (_Player.IsMoving())
        {
            if (collider.TryGetComponent(out ICombatable combatable))
            {
                _Animator.SetBool(_AnimatorHash, true);
                combatable.Damaged(StatTable[ItemStat.AttackPower], collider);
            }
        }
    }
    protected override void AttackAnimationPlayOver()
    {
        _Animator.SetBool(_AnimatorHash, false);

        base.AttackAnimationPlayOver();
    }
}
