using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RealWorldVRGame
{
    public class SimpleBazukaWeapon : WeaponBase
    {
        [SerializeField] private Transform _runSourcePoint;
        [SerializeField] private MovableBullet _missilePrefab;

        [SerializeField] private List<MovableBullet> _bulletsPool = new List<MovableBullet>();
        
        protected override void OnShot()
        {
            var deactivatedMissle = _bulletsPool.FirstOrDefault(b => !b.gameObject.activeInHierarchy);
            if (deactivatedMissle != null)
            {
                deactivatedMissle.gameObject.SetActive(true);
                deactivatedMissle.transform.position = _runSourcePoint.position;
                deactivatedMissle.transform.rotation = _runSourcePoint.rotation;
                deactivatedMissle.ResetBullet();
            }
            else
            {
                deactivatedMissle = CreateMissile();
                _bulletsPool.Add(deactivatedMissle);
            }
            deactivatedMissle.IsActive = true;
        }

        private MovableBullet CreateMissile()
        {
            var bullet = Instantiate(_missilePrefab);
            bullet.transform.position = _runSourcePoint.position;
            bullet.transform.rotation = _runSourcePoint.rotation;
            return bullet;
        }
    }
}

