using UnityEngine;

public class GreatSword : Item
{
    [SerializeField] private Animator Animator;
    [SerializeField] private Area AttackArea;

    [Header("Charge Ablity")]
    [SerializeField] private Projection SwordDance;
    [Range(0f, 1f)]
    [SerializeField] private float DemandCharge;
    [Range(1f, 20f)]
    [SerializeField] private float SwordDanceSpeed;

    private Pool<Projection> SwordDancePool;

    private int mAnimControlKey;
    private int mAnimPlayKey;

    private bool mIsAlreadyInit = false;

    private GameObject mPlayer;

    public override bool CanAttackState
    {
        get => Finger.Instance.Gauge.Charge >= DemandCharge;
    }

    public override void OffEquipThis(SlotType offSlot)
    {
        switch (offSlot)
        {
            case SlotType.Weapon:
                Inventory.Instance.ChargeAction -= ChargeAction;
                break;
        }
    }

    public override void OnEquipThis(SlotType onSlot)
    {
        switch (onSlot)
        {
            case SlotType.Weapon:
                {
                    Inventory.Instance.ChargeAction += ChargeAction;

                    if (!mIsAlreadyInit)
                    {
                        mAnimPlayKey    = Animator.GetParameter(0).nameHash;
                        mAnimControlKey = Animator.GetParameter(1).nameHash;

                        AttackArea.SetEnterAction(HitAction);

                        SwordDancePool = new Pool<Projection>();
                        SwordDancePool.Init(2, SwordDance, p =>
                        {
                            p.transform.localScale = Vector2.one * 1.5f;

                            p.SetAction(
                            o =>
                            {
                                if (o.TryGetComponent(out ICombatable combatable))
                                {
                                    combatable.Damaged(StatTable[ItemStat.AttackPower], mPlayer);
                                }
                            }, 
                            o => SwordDancePool.Add(o));
                        });
                        mPlayer = transform.parent.parent.gameObject;

                        mIsAlreadyInit = true;
                    }
                }
                break;
        }
    }

    private void ChargeAction(float charge)
    {
        if (charge >= DemandCharge)
        {
            Animator.SetBool(mAnimPlayKey, true);
            Animator.SetBool(mAnimControlKey, !Animator.GetBool(mAnimControlKey));
        }
    }

    private void SwordDanceShoot()
    {
        var direction = Vector2.right;
        
        if (mPlayer.transform.localRotation.eulerAngles.y > 0f)
        {
            direction = Vector2.left;
        }
        var swordDance = SwordDancePool.Get();

        swordDance.Shoot(transform.position, direction, SwordDanceSpeed);
        swordDance.transform.rotation = mPlayer.transform.localRotation;
    }

    private void HitAction(GameObject hitTarget)
    {
        if (hitTarget.TryGetComponent(out ICombatable combatable))
        {
            combatable.Damaged(StatTable[ItemStat.AttackPower], mPlayer);

            Inventory.Instance.OnAttackEvent(mPlayer, combatable);
        }
    }

    protected override void CameraShake()
    {
        MainCamera.Instance.Shake(0.29f, 3.5f);
    }
}
