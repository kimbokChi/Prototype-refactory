using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RobotsBossSword : MonoBehaviour, IObject, ICombatable
{
    private const int Idle   = 0;
    private const int Move   = 1;
    private const int Attack = 2;
    private const int Death  = 3;

    [SerializeField] private ItemDropper _ItemDropper;

    [Header("Ability")]
    [SerializeField] private AbilityTable _AbilityTable;
    [SerializeField] private Animator _Animator;
    [SerializeField] private GameObject _HealthBar;
    [SerializeField] private Image _HealthBarImage;

    [Header("Attack Property")]
    [SerializeField] private float _DownFallLength;

    private Player _Player;
    private int _AnimControlKey;

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
        EffectLibrary.Instance.UsingEffect(EffectKind.SwordAfterImage, transform.position + Vector3.down * 4.5f);
    }
    private IEnumerator DownFallRoutine()
    {
        var targetPos = transform.localPosition + Vector3.down * _DownFallLength;

        for (float i = 0; i < 0.167f; i += Time.deltaTime * Time.timeScale)
        {
            float rate = Mathf.Min(i / 0.167f, 1f);
            transform.localPosition = Vector2.Lerp(transform.localPosition, targetPos, rate);

            yield return null;
        }
    }
}
