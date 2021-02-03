using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrdinaryBow : Item
{
    [SerializeField] private Animator Animator;

    [Header("Arrow Info")]
    [SerializeField] private Projection Arrow;
    [SerializeField] private float ShootSpeed;
    [SerializeField] private Vector3 ShootOffset;

    private int mAnimPlayKey;

    private GameObject mPlayer;

    private Pool<Projection> mArrowPool;
    private bool mIsAlreadyInit = false;

    public override void AttackAction(GameObject attacker, ICombatable combatable)
    {
        mPlayer = attacker;

        Animator.SetBool(mAnimPlayKey, true);
    }

    public override void OffEquipThis(SlotType offSlot)
    {
        
    }

    public override void OnEquipThis(SlotType onSlot)
    {
        switch (onSlot)
        {
            case SlotType.Weapon:
                {
                    if (!mIsAlreadyInit)
                    {
                        mArrowPool = new Pool<Projection>();
                        mArrowPool.Init(3, Arrow, o =>
                        {
                            o.transform.parent = ItemStateSaver.Instance.transform;

                            o.SetAction(hit =>
                            {
                                if (hit.TryGetComponent(out ICombatable combatable))
                                {
                                    combatable.Damaged(StatTable[ItemStat.AttackPower], mPlayer);

                                    Inventory.Instance.OnAttackEvent(mPlayer, combatable);
                                    MainCamera.Instance.Shake();
                                }
                            },
                            p => mArrowPool.Add(p));
                        });

                        mAnimPlayKey = Animator.GetParameter(0).nameHash;
                        mIsAlreadyInit = true;
                    }
                }
                break;
        }
    }

    private void ShootArrow()
    {
        var direction = Vector2.right;

        if (mPlayer.transform.localRotation.eulerAngles.y > 0f)
        {
            direction = Vector2.left;
        }
        Projection arrow = mArrowPool.Get();

        MainCamera.Instance.Shake();

        arrow.transform.rotation = mPlayer.transform.localRotation;
        arrow.Shoot(transform.position + ShootOffset, direction, ShootSpeed);
    }

    protected override void AttackAnimationPlayOver()
    {
        base.AttackAnimationPlayOver();

        Animator.SetBool(mAnimPlayKey, false);
    }

    protected override void CameraShake()
    {
        MainCamera.Instance.Shake();
    }

    public override void AttackCancel()
    {
        Animator.SetBool(mAnimPlayKey, false);
    }
}
