using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrozenShose : Item
{
    [SerializeField] private Animator Animator;

    [Header("Pillar Section")]
    [SerializeField] private Area FrozenPillar;
    [SerializeField] private Vector2 PillarOffset;

    private Pool<Area> _PillarPool;
    private bool _IsAlreadyInit = false;

    private GameObject _Player;

    public override void AttackAction(GameObject attacker, ICombatable combatable)
    {
        _Player = attacker;

        Animator.SetBool(Animator.GetParameter(0).nameHash, true);
    }

    public override void OffEquipThis(SlotType offSlot)
    {
        // To do...
    }

    public override void OnEquipThis(SlotType onSlot)
    {
        Init();

        // To do...
    }

    private void Init()
    {
        if (!_IsAlreadyInit)
        {
            _PillarPool = new Pool<Area>();
            _PillarPool.Init(4, FrozenPillar, pillar =>
            {
                pillar.SetEnterAction(o => 
                {
                    if (o.TryGetComponent(out ICombatable combatable))
                    {

                        combatable.Damaged(StatTable[ItemStat.AttackPower], _Player);
                    }
                }); 
            });

            _IsAlreadyInit = true;
        }
    }

    // 애니메이션 이벤트로 실행된다
    private void SummonFrozenPillar(Vector2 summonPoint)
    {
        var pillar = _PillarPool.Get();

        pillar.transform.position = summonPoint + PillarOffset;
    }
}
