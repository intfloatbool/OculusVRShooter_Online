using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RealWorldVRGame.AI;
using UnityEngine;

namespace RealWorldVRGame
{
    public class UnitRangeAttack : MonoBehaviour
    {
        [SerializeField] private OnlineEnemyAi _enemyAi;
        [SerializeField] private Transform _attackPoint;
        [SerializeField] private MovableBullet _bulletPrefab;

        [Space(5f)] 
        [Header("Runtime references")]
        [SerializeField] private List<MovableBullet> _bulletsPool = new List<MovableBullet>();


        private void Awake()
        {
            _enemyAi.OnUnitAttack += OnAttack;
        }

        private void OnAttack(UnitStats unitStats)
        {
            if (unitStats != null)
            {
                _attackPoint.LookAt(unitStats.transform.position);
            }
            else
            {
                _attackPoint.LookAt(_enemyAi.CurrentTargetPositionRPC);
            }
            CreateBullet();
        }

        private void CreateBullet()
        {
            var activatedBullet = _bulletsPool.FirstOrDefault(b => !b.gameObject.activeInHierarchy);
            if (activatedBullet != null)
            {
                activatedBullet.gameObject.SetActive(true);
                activatedBullet.transform.position = _attackPoint.transform.position;
                activatedBullet.transform.rotation = _attackPoint.transform.rotation;
                activatedBullet.ResetBullet();
            }
            else
            {
                activatedBullet = Instantiate(_bulletPrefab);
                activatedBullet.transform.position = _attackPoint.transform.position;
                activatedBullet.transform.rotation = _attackPoint.transform.rotation;
                _bulletsPool.Add(activatedBullet);
            }

            activatedBullet.IsActive = true;
        }
    }
 
}
