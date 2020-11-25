using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kraken : MonoBehaviour, IObject
{
    private enum Pattern
    {
        FallingTentacle, StrikeTentacle, ArtilleryFire, SummonTentacle, SummonSeaMonster
    }
    [SerializeField] private AbilityTable AbilityTable;
    [SerializeField] private EnemyAnimator EnemyAnimator;

    [SerializeField] private GameObject[] SeaMonsters;

    [Header("ArtilleryFire Info")]
    [SerializeField] private Projection ArtilleryShell;
    [SerializeField] private float ArtilleryShellSpeed;
    [SerializeField] private float ArtilleryFireTime;
    [SerializeField] private float ArtilleryFireDelay;
    [SerializeField] private float RotationSpeed;

    private Pattern _NextPattern;
    private int _PatternInvokeCount;

    private AttackPeriod _AttackPeriod;
    private Pool<Projection> _ArtilleryShellPool;

    public void IInit()
    {
        _PatternInvokeCount = 0;
        //EnemyAnimator.Init();

        _ArtilleryShellPool = new Pool<Projection>();
        _ArtilleryShellPool.Init(16, ArtilleryShell, o => 
        {
            o.SetAction(
                hitTarget =>
                {
                    if (hitTarget.TryGetComponent(out ICombatable combatable))
                    {
                        combatable.Damaged(AbilityTable.AttackPower, gameObject);
                    }
                },
                projection => 
                {
                    _ArtilleryShellPool.Add(projection);
                });
        });

        _AttackPeriod = new AttackPeriod(AbilityTable);
        _AttackPeriod.SetAction(Period.Attack, () => 
        {
            _PatternInvokeCount++;

            if (_PatternInvokeCount == 3)
            {
                _NextPattern = Pattern.SummonSeaMonster;

                _PatternInvokeCount = 0;
            }
            else if (_PatternInvokeCount <= 3)
            {
                _NextPattern = Pattern.SummonTentacle;
            }
            else
            {
                _NextPattern = (Pattern)Random.Range(0, 3);
            }
            switch (_NextPattern)
            {
                case Pattern.FallingTentacle:
                    break;

                case Pattern.StrikeTentacle:
                    break;

                case Pattern.ArtilleryFire:
                    StartCoroutine(ArtilleryFire());
                    break;

                case Pattern.SummonTentacle:
                    SummonTentacle();
                    break;

                case Pattern.SummonSeaMonster:
                    break;
            }
        });
        // _AttackPeriod.StartPeriod();
    }

    public void IUpdate()
    {
        
    }

    private void Start()
    {
        IInit();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(ArtilleryFire());
        }
    }

    public void PlayerEnter(MESSAGE message, Player enterPlayer)
    {
        
    }

    public void PlayerExit(MESSAGE message)
    {
        
    }

    private void SummonTentacle()
    {


        _AttackPeriod.AttackActionOver();
    }

    private IEnumerator ArtilleryFire()
    {
        float speed = ArtilleryShellSpeed;

        float theta = 0f;

        for (float i = 0f; i < ArtilleryFireTime; i += ArtilleryFireDelay)
        {
            float angle = 0f;

            for (int j = 0; j < 4; j++)
            {
                angle += (theta + (90 * j));
                angle *= Mathf.Deg2Rad;

                var direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;

                _ArtilleryShellPool.Get().Shoot(transform.position, direction, speed);
            }
            yield return new WaitForSeconds(ArtilleryFireDelay); 
            
            theta += RotationSpeed;

            if (theta >= 360f) theta = 0f;
        }
        _AttackPeriod.AttackActionOver();
    }

    public GameObject ThisObject() => gameObject;
    public bool IsActive() => gameObject.activeSelf;
}
