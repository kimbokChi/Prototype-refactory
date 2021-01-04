using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinNormal : MonoBehaviour, IObject, ICombatable, IAnimEventReceiver
{
    [Header("Ability")]
    [SerializeField] private AbilityTable _AbilityTable;
    [SerializeField] private EnemyAnimator _EnemyAnimator;

    [Header("Modules")]
    [SerializeField] private MovementModule _Movement;
    [SerializeField] private RecognitionModule _Recognition;
    [SerializeField] private ShortRangeModule _AttackModule;

    public AbilityTable GetAbility => _AbilityTable;

    public void AnimationPlayOver(AnimState anim)
    {
        
    }
    public void Damaged(float damage, GameObject attacker)
    {
        EffectLibrary.Instance.UsingEffect(EffectKind.Damage, transform.position);

        _EnemyAnimator.ChangeState(AnimState.Damaged);
        if ((_AbilityTable.Table[Ability.CurHealth] -= damage) <= 0)
        {
            _AttackModule.SetActivePeriod(false);

            _Movement.MoveStop();
            _EnemyAnimator.ChangeState(AnimState.Death);
            HealthBarPool.Instance.UnUsingHealthBar(transform);
        }
    }

    public void IInit()
    {
        _EnemyAnimator.Init();
        HealthBarPool.Instance.UsingHealthBar(-1f, transform, _AbilityTable);

        _AttackModule.Init(gameObject, _AbilityTable);
        _AttackModule.RunningDrive();
        _AttackModule.SetPeriodAction(Period.Attack, () => 
        {
            _Movement.MoveStop();

            _EnemyAnimator.ChangeState(AnimState.Attack);
        });

        MovementModuleInit();
    }
    public void IUpdate()
    {
        if (_AbilityTable[Ability.CurHealth] > 0)
        {

            if (!_AttackModule.IsPeriodProgressing())
            {
                if (_AttackModule.RangeHasAny() && _Recognition.IsLookAtPlayer())
                {

                    _Movement.MoveStop();
                    _AttackModule.SetActivePeriod(true);
                }
            }
        }
    }
    private void MovementModuleInit()
    {
        _Movement.SetMovementEvent(_Recognition,
        () =>
        {
            _EnemyAnimator.ChangeState(AnimState.Move);
        },
        () =>
        {
            _EnemyAnimator.ChangeState(AnimState.Idle);
        });
        _Movement.SetMovementLogic(_Recognition,
        () =>
        {
            return _EnemyAnimator.CurrentState() == AnimState.Idle &&
                  !_AttackModule.IsPeriodProgressing();
        });
        _Movement.RunningDrive(_AbilityTable);
    }

    public void PlayerEnter(MESSAGE message, Player enterPlayer)
    {
        if (_AbilityTable.CanRecognize(message))
        {
            _Recognition.PlayerEnter(enterPlayer);
        }
    }
    public void PlayerExit(MESSAGE message)
    {
        if (_AbilityTable.CantRecognize(message))
        {
            _Recognition.PlayerExit();
        }
    }
    public bool IsActive()
    {
        return gameObject.activeSelf;
    }
    public GameObject ThisObject()
    {
        return gameObject;
    }
    public void CastBuff(Buff buffType, IEnumerator castedBuff)
    {
        StartCoroutine(castedBuff);
    }
}
