﻿using UnityEngine;

public class LongSword : Item
{
    [SerializeField] private Animator Animator;
    [SerializeField] private Area CollisionArea;
    [SerializeField] private SwordDance SwordDance;

    private int mAnimPlayKey;
    private int mAnimControlKey;

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
    {
        if (offSlot.Equals(SlotType.Weapon))
            Inventory.Instance.ChargeAction -= ChargeAction;
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

    private void CameraShake() {
        MainCamera.Instance.Shake();
    }

    private void HitAction(GameObject hitObject)
    {
        if (hitObject.TryGetComponent(out ICombatable combatable))
        {
            combatable.Damaged(1f, mPlayer);

            Inventory.Instance.OnAttackEvent(mPlayer, combatable);
        }
    }

    private void AnimationPlayOver()
    {
        CollisionArea.enabled = false;
        CollisionArea.GetCollider.enabled = false;
    }
}