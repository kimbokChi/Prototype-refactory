using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerousBottle : Item
{
    [SerializeField] private Animator _Animator;
    [SerializeField] private BoxCollider2D _Collider;

    [Header("Poision Property")]
    [SerializeField, Range( 1,  10)] private uint _PoisionLevel;
    [SerializeField, Range(0f, 10f)] private float _PoisionDurate;

    private int _AnimatorHash;
    private bool _IsAlreadyInit = false;

    private GameObject _Player;
    private void Init()
    {
        if (!_IsAlreadyInit)
        {
            _IsAlreadyInit = true;
            _AnimatorHash = _Animator.GetParameter(0).nameHash;

            _Collider.enabled = false;
        }
    }
    public override void AttackAction(GameObject attacker, ICombatable combatable)
    {
        _Player = attacker;
        _Animator.SetBool(_AnimatorHash, true);
    }
    private void EnableCollider()
    {
        float minX = _Collider.size.x - _Collider.offset.x;
        float maxX = _Collider.size.x + _Collider.offset.x;

        int invoke = (int)(_Collider.size.x * 16);
        for (int i = 0; i < invoke; i++)
        {
            Vector3 offset = new Vector2(Random.Range(minX, maxX), Random.value);
            EffectLibrary.Instance.UsingEffect(EffectKind.Poision, transform.position + offset);
        }
    }
    protected override void AttackAnimationPlayOver()
    {
        base.AttackAnimationPlayOver();

        _Animator.SetBool(_AnimatorHash, false);
    }
    public override void AttackCancel()
    {
        _Animator.SetBool(_AnimatorHash, false);
    }
    public override void OffEquipThis(SlotType offSlot)
    {
        
    }
    public override void OnEquipThis(SlotType onSlot)
    {
        Init();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (collision.TryGetComponent(out ICombatable combatable))
            {
                combatable.CastBuff(Buff.Poision, GetPoisionBuff(combatable));
                combatable.Damaged(StatTable[ItemStat.AttackPower], _Player);
            }
        }
    }
    private IEnumerator GetPoisionBuff(ICombatable combatable)
    {
        return BuffLibrary.Instance.GetBuff(Buff.Poision, _PoisionLevel, _PoisionDurate, combatable.GetAbility);
    }
}
