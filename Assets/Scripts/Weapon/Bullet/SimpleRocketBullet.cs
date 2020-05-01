using UnityEngine;

namespace RealWorldVRGame
{
    public class SimpleRocketBullet : MovableBullet
    {
        [SerializeField] protected int _minDamage;
        [SerializeField] protected int _maxDamage;
        protected int RandomDamage => Random.Range(_minDamage, _maxDamage);
        
        [SerializeField] protected float _raycastLength = 1f;
        [SerializeField] protected float _sphereRaidus = 0.5f;
        
        protected override void Update()
        {
            base.Update();
            if (_isActive)
            {
                SearchForTarget();
            }
        }
        
        protected void SearchForTarget()
        {
            RaycastHit hit;
            if(Physics.SphereCast(transform.position,_sphereRaidus, transform.forward, out hit, _raycastLength))
            {
                var hitGO = hit.collider.gameObject;
                var unitStats = hitGO.GetComponent<UnitStats>();
                if(unitStats == null)
                {
                    unitStats = hitGO.transform.root.GetComponent<UnitStats>();
                }
                OnBang(unitStats);
            }
        }

        protected override void OnBang(UnitStats unitStats = null)
        {
            base.OnBang(unitStats);
            if(unitStats != null)
            {
                unitStats.MakeDamage(RandomDamage);
            }
        }

    }
}
