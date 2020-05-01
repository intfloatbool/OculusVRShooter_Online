using UnityEngine;

namespace RealWorldVRGame
{
    public class PistolWeapon : WeaponBase
    {
        
        [SerializeField] private float _maxDistance = 60f;
        [SerializeField] private Transform _firePoint;
        [SerializeField] private int _minDamage = 2;
        [SerializeField] private int _maxDamage = 5;
        private int RandomDamage => Random.Range(_minDamage, _maxDamage);
        [SerializeField] private Transform _effectTransform;
        [SerializeField] private float _effectTime = 1f;
        private float _currentEffectTimer = 0;
        private bool _isActiveTimer;
        protected override void OnShot()
        {
            Debug.DrawRay(_firePoint.transform.position, _firePoint.forward * _maxDistance, Color.blue, 2f);
            RaycastHit hit;
            if (Physics.Raycast(_firePoint.position, _firePoint.forward, out hit, _maxDistance))
            {
                var unitStats = hit.collider.gameObject.GetComponent<UnitStats>();
                if (unitStats != null)
                {
                    unitStats.MakeDamage(RandomDamage);
                    var poolManager = EffectsPoolManager.Instance;
                    if (poolManager != null)
                    {
                        poolManager.ShowBloodEffect(hit.point, this.transform);
                    }
                }
            }
            _isActiveTimer = true;
        }

        protected override void Update()
        {
            base.Update();
            _effectTransform.gameObject.SetActive(_isActiveTimer);
            if (_isActiveTimer)
            {
                if (_currentEffectTimer > _effectTime)
                {
                    _currentEffectTimer = 0;
                    _isActiveTimer = false;
                }
                _currentEffectTimer += Time.deltaTime;
            }
        }
    }
}
