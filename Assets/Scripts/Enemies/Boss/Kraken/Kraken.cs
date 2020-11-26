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

    [Header("ArtilleryFire Info")]
    [SerializeField] private Projection ArtilleryShell;
    [SerializeField] private float ArtilleryShellSpeed;
    [SerializeField] private float ArtilleryFireTime;
    [SerializeField] private float ArtilleryFireDelay;
    [SerializeField] private float RotationSpeed;

    [Header("Summon Tentacle")]
    [SerializeField] private Tentacle Tentacle;
    [SerializeField] private int TentacleCount;
    private Pool<Tentacle> _TentaclePool;

    [Header("Summon SeaMonster")]
    [SerializeField] private GameObject[] SeaMonsters;

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

        _TentaclePool = new Pool<Tentacle>();
        _TentaclePool.Init(4, Tentacle, o => 
        {
            o.DeathrattleAction = tentacle =>
            {
                TentacleCount--;

                _TentaclePool.Add(tentacle);
            };
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
            else if (TentacleCount <= 3)
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
            SummonSeaMonster();

            //SummonTentacle();

            // StartCoroutine(ArtilleryFire());
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
        int floorIndex = Random.Range(0, 3);

        var tentacle = _TentaclePool.Get();

        var room = Castle.Instance.GetFloorRooms()[floorIndex];
            room.AddIObject(tentacle);

        tentacle.transform.parent = room.transform;

        floorIndex *= 3;
        float summonPointMinX = Castle.Instance.GetMovePoint((DIRECTION9)floorIndex).x;
        float summonPointMaxX = Castle.Instance.GetMovePoint((DIRECTION9)floorIndex + 2).x;

        Vector2 summonPoint = new Vector2(Random.Range(summonPointMinX, summonPointMaxX), 2.3f);
        tentacle.transform.localPosition = summonPoint;

        TentacleCount++;
        _AttackPeriod.AttackActionOver();
    }

    private void SummonSeaMonster()
    {
        int floorIndex = Random.Range(0, 3);

        var room = Castle.Instance.GetFloorRooms()[floorIndex];

        float summonPointMinX = Castle.Instance.GetMovePoint((DIRECTION9)(floorIndex * 3)).x;
        float summonPointMaxX = Castle.Instance.GetMovePoint((DIRECTION9)(floorIndex * 3) + 2).x;

        for (int i = 0; i < 3; i++)
        {
            Vector3 summonPoint = new Vector2(Random.Range(summonPointMinX, summonPointMaxX), 0f);

            var monster = Instantiate(SeaMonsters[Random.Range(0, SeaMonsters.Length)], room.transform);
                monster.transform.localPosition += summonPoint;

            if (monster.TryGetComponent(out IObject _object))
            {
                room.AddIObject(_object);
            }
        }
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
