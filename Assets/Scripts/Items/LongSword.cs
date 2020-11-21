using UnityEngine;

public class LongSword : Item
{
    [SerializeField] private Animator Animator;
    [SerializeField] private Area CollisionArea;
    [SerializeField] private SwordDance SwordDance;

    private int mAnimPlayKey;
    private int mAnimControlKey;

    private GameObject mPlayer;

    private void Reset()
    {
        Debug.Assert(TryGetComponent(out Animator));
    }
    public override void OffEquipThis(SlotType offSlot)
    {
        if (offSlot.Equals(SlotType.Weapon))
        {
            Inventory.Instance.ChargeAction -= ChargeAction;

            AttackOverAction = null;
        }
    }

    public override void OnEquipThis(SlotType onSlot)
    {
        if (onSlot.Equals(SlotType.Weapon))
        {
            CollisionArea.SetEnterAction(HitAction);

            mAnimPlayKey    = Animator.GetParameter(0).nameHash;
            mAnimControlKey = Animator.GetParameter(1).nameHash;

            Inventory.Instance.ChargeAction += ChargeAction;

            mPlayer = mPlayer ?? transform.parent.parent.gameObject;
        }
    }

    public override void AttackAction(GameObject attacker, ICombatable combatable)
    {
        CollisionArea.enabled = true;
        CollisionArea.GetCollider.enabled = true;

        Animator.SetBool(mAnimPlayKey, true);
        Animator.SetBool(mAnimControlKey, !Animator.GetBool(mAnimControlKey));

        mPlayer = attacker;
    }

    private void ChargeAction(float charge)
    {
        Vector2 direction = Vector2.right;
        SwordDance.transform.localRotation = Quaternion.Euler(Vector3.zero);

        if (mPlayer.transform.localRotation.y == -1)
        {
            SwordDance.transform.localRotation = Quaternion.Euler(Vector3.up * 180f);
            direction = Vector2.left;
        }
        SwordDance.transform.position = transform.position;
        SwordDance.Launch(direction);
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