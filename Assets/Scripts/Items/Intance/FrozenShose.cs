using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrozenShose : Item
{
    [SerializeField] private Animator Animator;

    [Header("Pillar Section")]
    [SerializeField] private Projection FrozenPillar;
    [SerializeField] private float PillarOffsetY;

    private Pool<Projection> _PillarPool;
    private bool _IsAlreadyInit = false;
    private int _AnimControlKey;

    private GameObject _Player;

    protected override void AttackAnimationPlayOver()
    {
        base.AttackAnimationPlayOver();

        Animator.SetBool(_AnimControlKey, false);
    }

    public override void AttackAction(GameObject attacker, ICombatable combatable)
    {
        _Player = attacker;

        Animator.SetBool(_AnimControlKey, true);
    }

    public override void OffEquipThis(SlotType offSlot)
    { }

    public override void OnEquipThis(SlotType onSlot)
    {
        Init();
    }

    private void Init()
    {
        if (!_IsAlreadyInit)
        {
            _PillarPool = new Pool<Projection>();
            _PillarPool.Init(4, FrozenPillar, pillar =>
            {
                pillar.SetAction(
                hit => {
                    if (hit.TryGetComponent(out ICombatable combatable))
                    {
                        combatable.Damaged(StatTable[ItemStat.AttackPower], _Player);
                    }
                },
                fro => {
                    _PillarPool.Add(pillar);
                }); 
            });
            _AnimControlKey = Animator.GetParameter(0).nameHash;

            _IsAlreadyInit = true;
        }
    }

    // 애니메이션 이벤트로 실행된다
    private void SummonFrozenPillar()
    {
        MainCamera.Instance.Shake(0.35f, 1.3f);

        var playerMovePoints = Castle.Instance.GetPlayerRoom().GetMovePoints();

        for (int i = 0; i < 2; i++) {

            for (int j = 0; j < 2; j++)
            {
                float pointX = Random.Range(playerMovePoints[i].x, playerMovePoints[i + 1].x);

                _PillarPool.Get().Shoot(new Vector3(pointX, _Player.transform.position.y + PillarOffsetY, 0f), Vector2.zero, 0f);
            }
        }
    }
}
