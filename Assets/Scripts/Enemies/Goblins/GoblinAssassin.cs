using System.Collections;
using UnityEngine;

public class GoblinAssassin : MonoBehaviour, IObject, ICombatable, IAnimEventReceiver
{
    [Header("Ability")]
    [SerializeField] private AbilityTable _AbilityTable;
    [SerializeField] private EnemyAnimator _EnemyAnimator;

    [Header("Modules")]
    [SerializeField] private MovementModule _Movement;
    [SerializeField] private RecognitionModule _Recognition;
    [SerializeField] private ShortRangeModule _AttackModule;

    [Header("Dash")]
    [SerializeField] private float _DashDistance;
    [SerializeField] private float _DashSpeedScale;
    [SerializeField] private GameObject _DashEffect;

    private Coroutine _DashRoutine;

    public AbilityTable GetAbility => _AbilityTable;

    public void AnimationPlayOver(AnimState anim)
    {
        switch (anim)
        {
            case AnimState.Attack:
                {
                    _DashRoutine.StopRoutine();

                    _EnemyAnimator.ChangeState(AnimState.Idle);

                    _AttackModule.PeriodAttackPartOver();
                }
                break;

            case AnimState.Damaged:
                {
                    if (_Movement.IsMoving())
                    {
                        _EnemyAnimator.ChangeState(AnimState.Move);
                    }
                    else
                        _EnemyAnimator.ChangeState(AnimState.Idle);
                }
                break;

            case AnimState.Death:
                gameObject.SetActive(false);
                break;
        }
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

        _DashRoutine = new Coroutine(this);

        _AttackModule.Init(gameObject, _AbilityTable);
        _AttackModule.RunningDrive();
        _AttackModule.SetPeriodAction(Period.Begin, () =>
        {
            _Movement.MoveStop();

            _EnemyAnimator.ChangeState(AnimState.AttackBegin);
        });
        _AttackModule.SetPeriodAction(Period.Attack, () =>
        {
            _DashRoutine.StartRoutine(Dash());

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

    private IEnumerator Dash() 
    {
        Vector2 force;

        if (_Recognition.IsLookAtLeft)
        {
            force = Vector2.left * _DashDistance;
        }
        else
            force = Vector2.right * _DashDistance;

        Vector2 dashPoint = (Vector2)transform.localPosition + force;
                dashPoint.x.Range(-3.5f, 3.5f);

        _DashEffect.SetActive(true);
        _DashEffect.transform.parent = null;

        float ratio = 0;

        while(ratio < 1)
        {
            float Speed()
            {
                return Time.timeScale * Time.deltaTime * _DashSpeedScale * _AbilityTable.MoveSpeed;
            }
            ratio = Mathf.Min(1f, ratio + Speed());

            transform.localPosition = Vector2.Lerp(transform.localPosition, dashPoint, ratio);

            yield return null;
        }
        _DashRoutine.Finish();

        _DashEffect.SetActive(false);
        _DashEffect.transform.parent = transform;
        _DashEffect.transform.localPosition = Vector2.zero;

        AnimationPlayOver(AnimState.Attack);
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
