using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RobotsBossShield : MonoBehaviour, IObject, ICombatable
{
    private const float OnActiveSpeed = 5f;
    private const float ActiveEndTime = 0.571f;

    private const float RestBeginTime = 1.0f;
    private const float RestEndTime = 0.917f;

    private const int Idle   = 0;
    private const int Active = 1;
    private const int Death  = 2;
    private const int Rest   = 3;

    private readonly UnitizedPos[] Directions = new UnitizedPos[4]
    {
        UnitizedPos.TOP,
        UnitizedPos.MID_LEFT,
        UnitizedPos.MID_RIGHT,
        UnitizedPos.BOT
    };

    [SerializeField] private ItemDropper _ItemDropper;

    [Header("Ability")]
    [SerializeField] private AbilityTable _AbilityTable;
    [SerializeField] private Animator _Animator;
    [SerializeField] private Image _HealthBarImage;

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

    [ContextMenu("IdleOrder")]
    private void IdleOrder()
    {
        _Animator.SetInteger(_AnimControlKey, Idle);
    }
    [ContextMenu("ActiveOrder")]
    private void ActiveOrder()
    {
        _Animator.SetInteger(_AnimControlKey, Active);
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
        EffectLibrary.Instance.UsingEffect(EffectKind.Damage, transform.position);

        if ((_AbilityTable.Table[Ability.CurHealth] -= damage) <= 0f)
        {
            _ItemDropper.CoinDrop(40);
            _ItemDropper.TryPotionDrop(PotionName.SHealingPotion, PotionName.LHealingPotion);
        }
        float rate = _AbilityTable[Ability.CurHealth] / _AbilityTable[Ability.MaxHealth];
        _HealthBarImage.fillAmount = rate;
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
    private IEnumerator UpdateRoutine()
    {
        while (_AbilityTable[Ability.CurHealth] > 0f)
        {
            float moveWait = Random.Range(_MoveTimeMin, _MoveWaitMax);
            for (float i = 0f; i < moveWait; i += Time.deltaTime * Time.timeScale)
                yield return null;

            yield return StartCoroutine(MoveRoutine());

            float wait = _AbilityTable.BeginAttackDelay;
            for (float i = 0f; i < wait; i += Time.deltaTime * Time.timeScale)
                yield return null;

            _Animator.SetInteger(_AnimControlKey, Active);
            {
                Vector2 moveDirection;
                UnitizedPos startPos = Directions[Random.Range(0, 3)];
                
                var enumator = OnActiveDirection(startPos).GetEnumerator();

                enumator.MoveNext();
                moveDirection = enumator.Current;

                while (!enumator.Current.Equals(Vector2.zero))
                {
                    Vector2 pos = transform.localPosition;
                    pos += moveDirection * OnActiveSpeed * _AbilityTable.MoveSpeed * Time.deltaTime * Time.timeScale;

                    if ((pos.x > _MoveRange.x) || (pos.x < -_MoveRange.x) ||
                        (pos.y > _MoveRange.y) || (pos.y < -_MoveRange.y))
                    {
                        enumator.MoveNext();
                        moveDirection = enumator.Current;

                        continue;
                    }
                    transform.localPosition = pos;
                    yield return null;
                }
                _Animator.SetInteger(_AnimControlKey, Idle);
            }
            _RestCount++;

            for (float i = 0f; i < ActiveEndTime; i += Time.timeScale * Time.deltaTime)
                yield return null;

            if (_RestCount == 1)
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

    private IEnumerable<Vector2> OnActiveDirection(UnitizedPos start)
    {
        switch (start)
        {
            case UnitizedPos.TOP:
                {
                    yield return Vector2.up;
                    yield return Vector2.right;
                    yield return Vector2.down;
                    yield return Vector2.left;
                    yield return Vector2.up;
                    yield return Vector2.right;
                    yield return Vector2.zero;
                }
                break;
            case UnitizedPos.MID_LEFT:
                {
                    yield return Vector2.left;
                    yield return Vector2.up;
                    yield return Vector2.right;
                    yield return Vector2.down;
                    yield return Vector2.left;
                    yield return Vector2.up;
                    yield return Vector2.zero;
                }
                break;
            case UnitizedPos.MID_RIGHT:
                {
                    yield return Vector2.right;
                    yield return Vector2.down;
                    yield return Vector2.left;
                    yield return Vector2.up;
                    yield return Vector2.right;
                    yield return Vector2.down;
                    yield return Vector2.zero;
                }
                break;
            case UnitizedPos.BOT:
                {
                    yield return Vector2.down;
                    yield return Vector2.left;
                    yield return Vector2.up;
                    yield return Vector2.right;
                    yield return Vector2.down;
                    yield return Vector2.left;
                    yield return Vector2.zero;
                }
                break;

            default: yield break;
        }
    }
}
