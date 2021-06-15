using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotsBossGunner : MonoBehaviour, IObject, ICombatable
{
    private readonly Vector3 LookRightScaleBody = new Vector3(-1f, 1f, 1f);
    private readonly Vector3 LookRightScaleArm = new Vector3(-1f, -1f, 1f);

    private const float ActiveEndTime = 0.571f;

    private const float RestBeginTime = 1.0f;
    private const float RestEndTime = 0.917f;

    private const int Arm_Idle   = 0;
    private const int Arm_Rest   = 1;
    private const int Arm_Attack = 2;
    private const int Arm_Move   = 3;
    private const int Arm_Death  = 4;

    private const int Body_Idle  = 0;
    private const int Body_Rest  = 1;
    private const int Body_Death = 2;

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
    [SerializeField] private Animator _BodyAnimator;
    [SerializeField] private Animator _ArmAnimator;

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

    [Header("General Property")]
    [SerializeField] private Transform _FrontArmAxis;
    [SerializeField] private Transform _BehindArmAxis;

    private Player _Player;
    private int _BodyControlKey;
    private int _ArmControlKey;

    private int _RestCount;

    [ContextMenu("IdleOrder")]
    private void IdleOrder()
    {
        _BodyAnimator.SetInteger(_BodyControlKey, Body_Idle);
        _ArmAnimator.SetInteger(_ArmControlKey, Arm_Idle);
    }
    [ContextMenu("AttackOrder")]
    private void AttackOrder()
    {
        _BodyAnimator.SetInteger(_BodyControlKey, Body_Idle);
        _ArmAnimator.SetInteger(_ArmControlKey, Arm_Attack);
    }
    [ContextMenu("RestOrder")]
    private void RestOrder()
    {
        _BodyAnimator.SetInteger(_BodyControlKey, Body_Rest);
        _ArmAnimator.SetInteger(_ArmControlKey, Arm_Rest);
    }
    [ContextMenu("DeathOrder")]
    private void DeathOrder()
    {
        _BodyAnimator.SetInteger(_BodyControlKey, Body_Death);
        _ArmAnimator.SetInteger(_ArmControlKey, Arm_Death);
    }
    private void Awake()
    {
        IInit();
    }
    public void IInit()
    {
        _BodyControlKey = _BodyAnimator.GetParameter(0).nameHash;
        _ArmControlKey = _ArmAnimator.GetParameter(0).nameHash;

        StartCoroutine(UpdateRoutine());
    }
    private void Update()
    {
        Vector2 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        
        float rot = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 180f;
        Quaternion rotation;

        if (-90 > rot && rot > -270)
        {
            transform.localScale = LookRightScaleBody;

             _FrontArmAxis.localScale = LookRightScaleArm;
            _BehindArmAxis.localScale = LookRightScaleArm;

            rotation = Quaternion.AngleAxis(-rot, Vector3.forward);
        }
        else
        {
            transform.localScale = Vector3.one;

             _FrontArmAxis.localScale = Vector3.one;
            _BehindArmAxis.localScale = Vector3.one;

            rotation = Quaternion.AngleAxis(rot, Vector3.forward);
        }
         _FrontArmAxis.localRotation = rotation;
        _BehindArmAxis.localRotation = rotation;
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
        IdleOrder();
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

            AttackOrder();
            {
                
            }
            _RestCount++;

            for (float i = 0f; i < ActiveEndTime; i += Time.timeScale * Time.deltaTime)
                yield return null;

            if (_RestCount == 3)
            {
                _RestCount = 0;

                RestOrder();

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
                IdleOrder();

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
        IdleOrder();
    }
}
