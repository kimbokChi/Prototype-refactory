using System.Linq;
using UnityEngine;

// 코드가 매우 비효율적으로 작성되어있음!
public class MysteriousMace : Item
{
    [SerializeField] private TracerBullet TracerBullet;

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

            // 30%
            if (Random.value <= 0.3f)
            {
                Vector2 playerPos = mPlayer.transform.position;

                Instantiate(TracerBullet, playerPos, Quaternion.identity).Shoot(FindCloestEnemy(), TargetHitAction);
            }
        }
    }

    private void TargetHitAction(GameObject target)
    {
        if (target.TryGetComponent(out ICombatable combat))
        {
            // 2/3의 피해
            combat.Damaged(StatTable.Table[ItemStat.AttackPower] * 0.666f, mPlayer);
        }
    }

    private Transform FindCloestEnemy()
    {
        float Distance(GameObject o)
        {
            return Vector2.Distance(o.transform.position, mPlayer.transform.position);
        }
        var enemies = GameObject.FindGameObjectsWithTag("Enemy")
            .Where(o => o.activeSelf)
            .Where(o => Distance(o) > WeaponRange);

        var enemy = enemies.OrderBy(o => Distance(o)).FirstOrDefault();

        return enemy?.transform;
    }

    protected override void CameraShake()
    {
        MainCamera.Instance.Shake(0.25f, 1.5f, true);
    }
}
