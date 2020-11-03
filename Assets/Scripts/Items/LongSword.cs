using UnityEngine;

public class LongSword : Item
{
    [SerializeField] private Animator Animator;
    [SerializeField] private Area CollisionArea;

    private int mAnimPlayKey;
    private int mAnimControlKey;

    private bool mCanAttack;

    private GameObject mPlayer;

    public override float AttackTime
    { get => 0.245f; }
    public override float WeaponRange
    { get => 1.5f; }
    public override ItemRating Rating
    { 
        get => ItemRating.Rare; 
    }

    private void Reset()
    {
        Debug.Assert(TryGetComponent(out Animator));
    }

    public override void OffEquipThis(SlotType offSlot)
    { }

    public override void OnEquipThis(SlotType onSlot)
    {
        if (onSlot.Equals(SlotType.Weapon))
        {
            CollisionArea.SetEnterAction(HitAction);

            mAnimPlayKey    = Animator.GetParameter(0).nameHash;
            mAnimControlKey = Animator.GetParameter(1).nameHash;
        }
    }

    public override void AttackAction(GameObject attacker, ICombatable combatable)
    {
        mCanAttack = true;
        CollisionArea.enabled = true;

        Animator.SetBool(mAnimPlayKey, true);
        Animator.SetBool(mAnimControlKey, !Animator.GetBool(mAnimControlKey));

        mPlayer = attacker;
    }

    private void HitAction(GameObject hitObject)
    {
        if (mCanAttack) {
            if (hitObject.TryGetComponent(out ICombatable combatable))
            {
                combatable.Damaged(1f, mPlayer);

                Inventory.Instance.OnAttackEvent(mPlayer, combatable);
            }
        }
    }

    private void AnimationPlayOver()
    {
        mCanAttack = false;
        CollisionArea.enabled = false;
    }
}