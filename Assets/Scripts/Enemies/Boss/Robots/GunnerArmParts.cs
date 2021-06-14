using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunnerArmParts : MonoBehaviour
{
    [SerializeField] private Projection _Bullet;
    [SerializeField] private GameObject _Owner;
    [SerializeField] private Transform _Arm;
    [SerializeField] private AbilityTable _Ability;

    [SerializeField] private Transform _LShootPoint;
    [SerializeField] private Transform _RShootPoint;

    [SerializeField] private float _ShootSpeed;

    private Pool<Projection> _BulletPool;
    private ICombatable _PlayerCombat;

    private bool _IsAlreadyInit = false;

    public void ShootingBullet()
    {
        LazyInit();
        float eulerAngleZ = _Arm.rotation.eulerAngles.z * Mathf.Deg2Rad + Mathf.PI;

        Quaternion rotation = Quaternion.AngleAxis(eulerAngleZ * Mathf.Rad2Deg - 90, Vector3.forward);
        Vector2 direction = new Vector2(Mathf.Cos(eulerAngleZ), Mathf.Sin(eulerAngleZ));

        var bullet = _BulletPool.Get();
        bullet.Shoot(_LShootPoint.position, direction, _ShootSpeed);
        bullet.transform.rotation = rotation;

        bullet = _BulletPool.Get();
        bullet.Shoot(_RShootPoint.position, direction, _ShootSpeed);
        bullet.transform.rotation = rotation;
    }
    private void LazyInit()
    {
        if (!_IsAlreadyInit)
        {
            _BulletPool = new Pool<Projection>();
            _BulletPool.Init(10, _Bullet, o =>
            {
                o.SetAction(
                    hit => 
                    {
                        if (hit.CompareTag("Player"))
                        {
                            if (_PlayerCombat == null)
                            {
                                hit.TryGetComponent(out _PlayerCombat);
                            }
                            _PlayerCombat.Damaged(_Ability.AttackPower, _Owner);
                            MainCamera.Instance.Shake(0.1f, 0.8f);
                        }
                    }, 
                    bullet => 
                    {
                        _BulletPool.Add(bullet);
                    });
            });

            _IsAlreadyInit = true;
        }
    }
}
