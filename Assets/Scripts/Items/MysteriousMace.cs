using UnityEngine;

public class MysteriousMace : Item
{
    [SerializeField] private Animator Animator;
    [SerializeField] private Area CollisionArea;

    private int mAnimPlayKey;
    private int mAnimControlKey;

    private GameObject mPlayer;

    private void Reset()
    {
        Debug.Assert(TryGetComponent(out Animator));
    }

    public override void AttackAction(GameObject attacker, ICombatable combatable)
    {
        Animator.SetBool(mAnimPlayKey, true);
        Animator.SetBool(mAnimControlKey, !Animator.GetBool(mAnimControlKey));

        mPlayer = attacker;
    }

    public override void OffEquipThis(SlotType offSlot)
    {
        
    }

    public override void OnEquipThis(SlotType onSlot)
    {
        if (onSlot.Equals(SlotType.Weapon))
        {
            CollisionArea.SetEnterAction(HitAction);

            mAnimPlayKey    = Animator.GetParameter(0).nameHash;
            mAnimControlKey = Animator.GetParameter(1).nameHash;

            mPlayer = transform.parent.parent.gameObject;
        }
    }

    private void HitAction(GameObject target)
    {
        if (target.TryGetComponent(out ICombatable combatable))
        {
            combatable.Damaged(StatTable.Table[ItemStat.AttackPower], mPlayer);

            Inventory.Instance.OnAttackEvent(mPlayer, combatable);
        }
    }

    protected override void CameraShake()
    {
        MainCamera.Instance.Shake(0.25f, 1.5f, true);
    }
}
