using UnityEngine;

public class LongSword : Item
{
    [SerializeField] private Animator Animator;
    [SerializeField] private Area CollisionArea;

    private int _AnimPlayKey;
    private int _AnimControlKey;

    private GameObject mPlayer;

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

        mPlayer = attacker;
    }
    private void HitAction(GameObject hitObject)
    {
        if (hitObject.TryGetComponent(out ICombatable combatable))
        {
            combatable.Damaged(StatTable[ItemStat.AttackPower], mPlayer);

            Inventory.Instance.OnAttackEvent(mPlayer, combatable);
        }
    }
}