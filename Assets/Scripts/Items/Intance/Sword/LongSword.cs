using UnityEngine;

public class LongSword : Item
{
    [SerializeField] private Animator Animator;
    [SerializeField] private Area CollisionArea;

    private int _AnimPlayKey;
    private int _AnimControlKey;

    private GameObject mPlayer;
    private bool _IsAttackOver = true;

    private void Reset()
    {
        Debug.Assert(TryGetComponent(out Animator));
    }
    public override void OffEquipThis(SlotType offSlot)
    {
        if (offSlot.Equals(SlotType.Weapon))
        {
            AttackOverAction = null;
        }
    }
    protected override void AttackAnimationPlayOver()
    {
        base.AttackAnimationPlayOver();

        _IsAttackOver = true;
    }
    public override void OnEquipThis(SlotType onSlot)
    {
        if (onSlot.Equals(SlotType.Weapon))
        {
            CollisionArea.SetEnterAction(HitAction);

            _AnimPlayKey    = Animator.GetParameter(0).nameHash;
            _AnimControlKey = Animator.GetParameter(1).nameHash;

            mPlayer = mPlayer ?? transform.parent.parent.gameObject;
        }
    }

    public override void AttackAction(GameObject attacker, ICombatable combatable)
    {
        CollisionArea.enabled = true;
        CollisionArea.GetCollider.enabled = true;

        Animator.SetBool(_AnimPlayKey, true);
        Animator.SetBool(_AnimControlKey, !Animator.GetBool(_AnimControlKey));

        _IsAttackOver = false;

        mPlayer = attacker;
    }
    protected override void CameraShake()
    {
        MainCamera.Instance.Shake(0.3f, 0.3f);
    }
    private void HitAction(GameObject hitObject)
    {
        if (hitObject.TryGetComponent(out ICombatable combatable))
        {
            combatable.Damaged(StatTable[ItemStat.AttackPower], mPlayer);

            Inventory.Instance.OnAttackEvent(mPlayer, combatable);
        }
    }

    public override void AttackCancel()
    {
        if (!_IsAttackOver)
        {
            Animator.SetBool(_AnimPlayKey, false);
            Animator.SetBool(_AnimControlKey, true);
        }
    }
}