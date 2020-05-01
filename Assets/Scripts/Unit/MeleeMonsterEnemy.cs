using UnityEngine;

namespace RealWorldVRGame.AI
{
    public class MeleeMonsterEnemy : OnlineEnemyAi
    {
        [Space(5f)]
        [Header("Melee unit properties")]
        [SerializeField] private Transform _attackingTransform;
        [SerializeField] private float _attackSphereRadius = 0.3f;

        protected override void Start()
        {
            base.Start();
            Debug.Assert(_attackingTransform != null, "_attackingTransform != null");
        }

        protected override void OnAttack()
        {
            base.OnAttack();
            RaycastHit hit;
            if (Physics.SphereCast(_attackingTransform.position, _attackSphereRadius, _attackingTransform.forward,
                out hit, _attackRange))
            {
                var unitStats = hit.collider.GetComponentInChildren<UnitStats>();
                if (unitStats != null)
                {
                    Debug.Assert(unitStats != _unitStats, "unitStats != _unitStats himself");
                    unitStats.MakeDamage(RandomDamage);
                    Debug.Log($"Melee monster make damage to: {unitStats.gameObject.name}!");
                    var poolManager = EffectsPoolManager.Instance;
                    if (poolManager != null)
                    {
                        poolManager.ShowBloodEffect(hit.point, this.transform);
                    }
                }
            }
        }
    }
}

