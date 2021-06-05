using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RobotsBossSword : MonoBehaviour, IObject, ICombatable
{
    private const float MoveAfterAnimation = 0.75f;

    private const float SplittingTime = 0.167f;
    private const float TurningTimeScale = 0.35f;

    private const float RestBeginTime = 1.0f;
    private const float RestEndTime = 0.75f;

    private const int Idle   = 0;
    private const int Move   = 1;
    private const int Attack = 2;
    private const int Death  = 3;
    private const int Rest   = 4;

    [SerializeField] private ItemDropper _ItemDropper;

    [Header("Ability")]
    [SerializeField] private AbilityTable _AbilityTable;
    [SerializeField] private Animator _Animator;
    [SerializeField] private GameObject _HealthBar;
    [SerializeField] private Image _HealthBarImage;

    [Header("Attack Property")]
    [SerializeField] private float _SplittingAccel;
    [SerializeField] private AnimationCurve _TurningCurve;

    [Header("Move Property")]
    [SerializeField] private Vector2 _MoveRange;
    [Space()]
    [SerializeField] private float _MoveWaitMin;
    [SerializeField] private float _MoveWaitMax;
    [Space()]
    [SerializeField] private float _MoveTimeMin;
    [SerializeField] private float _MoveTimeMax;

    [Header("Rest Property")]
    [SerializeField] private float _RestTime;
    [SerializeField] private AnimationCurve _FallingCurve;

    private Player _Player;
    private int _AnimControlKey;

    private int _RestCount;

    private Vector2 _AttackDirection;

    [ContextMenu("MoveOrder")]
    private void MoveOrder()
    {
        _Animator.SetInteger(_AnimControlKey, Move);
    }
    [ContextMenu("IdleOrder")]
    private void IdleOrder()
    {
        _Animator.SetInteger(_AnimControlKey, Idle);
    }
    [ContextMenu("AttackOrder")]
    private void AttackOrder()
    {
        _Animator.SetInteger(_AnimControlKey, Attack);
    }

    private void Awake()
    {
        IInit();
    }
    public void IInit()
    {
        _AnimControlKey = _Animator.GetParameter(0).nameHash;

        StartCoroutine(UpdateRoutine());
    }
    public void IUpdate()
    {
        
    }
    public void PlayerEnter(MESSAGE message, Player enterPlayer)
    {
        if (_AbilityTable.CanRecognize(message))
        {
            _Player = enterPlayer;
        }
    }
    public void PlayerExit(MESSAGE message)
    {
        if (_AbilityTable.CantRecognize(message))
        {
            _Player = null;
        }
    }
    public void Damaged(float damage, GameObject attacker)
    {
        if ((_AbilityTable.Table[Ability.CurHealth] -= damage) <= 0f)
        {
            _ItemDropper.CoinDrop(40);
            _ItemDropper.TryPotionDrop(PotionName.SHealingPotion, PotionName.LHealingPotion);
        }
    }
    #region
    public void CastBuff(Buff buffType, IEnumerator castedBuff)
    {
        StartCoroutine(castedBuff);
    }
    public bool IsActive()
    {
        return gameObject.activeSelf;
    }
    public GameObject ThisObject()
    {
        return gameObject;
    }
    public AbilityTable GetAbility => _AbilityTable;
    #endregion;

    private void AE_SetIdleState()
    {
        _Animator.SetInteger(_AnimControlKey, Idle);
    }
    private void AE_DownFall()
    {
        StartCoroutine(DownFallRoutine());

        float angle = Mathf.Atan2(_AttackDirection.y, _AttackDirection.x) * Mathf.Rad2Deg;

        EffectLibrary.Instance.UsingEffect(EffectKind.SwordAfterImage, transform.position + (Vector3)(_AttackDirection * 4.5f)).transform.rotation 
            = Quaternion.AngleAxis(angle - 270f, Vector3.forward);
    }
    private void AE_MoveAction()
    {
        StartCoroutine(MoveRoutine());
    }
    private IEnumerator DownFallRoutine()
    {
        float speed = _AbilityTable.MoveSpeed * SplittingTime * _SplittingAccel;
        Vector3 dir = _AttackDirection;
        Vector3 pos;

        dir.Normalize();
        for (float i = 0; i < SplittingTime; i += Time.deltaTime * Time.timeScale)
        {
            pos = transform.localPosition + dir * speed * Time.timeScale;

            if (_MoveRange.x < pos.x || -_MoveRange.x > pos.x || 
                _MoveRange.y < pos.y || -_MoveRange.y > pos.y) 
            {
                break; 
            }
            transform.localPosition = pos;
            yield return null;
        }
    }
    private IEnumerator UpdateRoutine()
    {
        while (_AbilityTable[Ability.CurHealth] > 0f)
        {
            float moveWait = Random.Range(_MoveTimeMin, _MoveWaitMax);
            for (float i = 0f; i < moveWait; i += Time.deltaTime * Time.timeScale)
                yield return null;

            _Animator.SetInteger(_AnimControlKey, Move);
            while (_Animator.GetInteger(_AnimControlKey) != Idle) yield return null;

            float wait = _AbilityTable.BeginAttackDelay + MoveAfterAnimation;
            for (float i = 0f; i < wait; i += Time.deltaTime * Time.timeScale)
                yield return null;

            // Attack and AttackTurning
            {
                Vector3 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

                _AttackDirection = dir.normalized;

                var rotA = transform.rotation;
                var rotB = Quaternion.AngleAxis(angle - 180f, Vector3.forward);

                AttackOrder();

                float time = Mathf.Abs(rotB.z - transform.rotation.z) * TurningTimeScale;
                for (float i = 0f; i < time; i += Time.timeScale * Time.deltaTime)
                {
                    float rate = _TurningCurve.Evaluate(Mathf.Min(i / time, 1f));

                    transform.rotation = Quaternion.Lerp(rotA, rotB, rate);
                    yield return null;
                }
            }
            while (_Animator.GetInteger(_AnimControlKey) != Idle) yield return null;
            {
                var rotA = transform.rotation;
                var rotB = Quaternion.identity;

                float time = Mathf.Abs(rotB.z - transform.rotation.z * TurningTimeScale);
                for (float i = 0f; i < time; i += Time.timeScale * Time.deltaTime)
                {
                    float rate = _TurningCurve.Evaluate(Mathf.Min(i / time, 1f));

                    transform.rotation = Quaternion.Lerp(rotA, rotB, rate);
                    yield return null;
                }
            }
            _RestCount++;

            if (_RestCount == 3)
            {
                _RestCount = 0;

                _Animator.SetInteger(_AnimControlKey, Rest);

                Vector2 point = Vector3.one * 1000f;
                Vector2 position = transform.position;

                for (UnitizedPos pos = UnitizedPos.TOP; pos < UnitizedPos.END; pos += 3)
                {
                    Vector2 p = Castle.Instance.GetMovePoint(pos);
                    if (pos != UnitizedPos.BOT && p.y > position.y) continue;

                    if ((p - position).sqrMagnitude < point.sqrMagnitude)
                    {
                        point = p;
                    }
                }
                point.x = position.x;
                for (float i = 0f; i < RestBeginTime; i += Time.deltaTime * Time.timeScale)
                {
                    float rate = _FallingCurve.Evaluate(Mathf.Min(i / RestBeginTime, 1f));

                    transform.position = Vector3.Lerp(position, point, rate);
                    yield return null;
                }
                for (float i = 0f; i < _RestTime; i += Time.deltaTime * Time.timeScale)
                    yield return null;
                _Animator.SetInteger(_AnimControlKey, Idle);

                Vector2 start = position = transform.localPosition;
                if ((position.x > _MoveRange.x) || (position.x < -_MoveRange.x) ||
                    (position.y > _MoveRange.y) || (position.y < -_MoveRange.y))
                {
                    position.x = Mathf.Clamp(position.x, -_MoveRange.x, _MoveRange.x);
                    position.y = Mathf.Clamp(position.y, -_MoveRange.y, _MoveRange.y);

                    for (float i = 0f; i < RestEndTime; i += Time.deltaTime * Time.timeScale)
                    {
                        float rate = _FallingCurve.Evaluate(Mathf.Min(i / RestEndTime, 1f));

                        transform.localPosition = Vector3.Lerp(start, position, rate);
                        yield return null;
                    }
                }
            }
        }
    }
    private IEnumerator MoveRoutine()
    {
        Vector3 direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        {
            float xAbsSub = Mathf.Abs(transform.localPosition.x - _MoveRange.x);
            float xAbsAdd = Mathf.Abs(transform.localPosition.x + _MoveRange.x);

            direction.x *=
                ((xAbsSub > xAbsAdd) && direction.x < 0) ||
                ((xAbsSub < xAbsAdd) && direction.x > 0) ? -1 : 1;

            float yAbsSub = Mathf.Abs(transform.localPosition.y - _MoveRange.y);
            float yAbsAdd = Mathf.Abs(transform.localPosition.y + _MoveRange.y);

            direction.y *=
                ((yAbsSub > yAbsAdd) && direction.y < 0) ||
                ((yAbsSub < yAbsAdd) && direction.y > 0) ? -1 : 1;
        }
        float moveTime = Random.Range(_MoveTimeMin, _MoveTimeMax);

        for (float i = 0f; i < moveTime; i += Time.deltaTime * Time.timeScale)
        {
            Vector3 pos = transform.localPosition;
            pos += direction * _AbilityTable.MoveSpeed * Time.deltaTime * Time.timeScale;

            if ((pos.x > _MoveRange.x) || (pos.x < -_MoveRange.x) ||
                (pos.y > _MoveRange.y) || (pos.y < -_MoveRange.y))
            {
                break;
            }
            transform.localPosition = pos;
            yield return null;
        }
        _Animator.SetInteger(_AnimControlKey, Idle);
    }
}
