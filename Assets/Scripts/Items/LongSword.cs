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
    { get => 0.35f; }
    public override float WeaponRange
    { get => 1.2f; }
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
        CollisionArea.GetCollider.enabled = true;

        Animator.SetBool(mAnimPlayKey, true);
        Animator.SetBool(mAnimControlKey, !Animator.GetBool(mAnimControlKey));

        mPlayer = attacker;
    }

    private void CameraShake() {
        MainCamera.Instance.Shake();
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
        CollisionArea.GetCollider.enabled = false;
    }
}