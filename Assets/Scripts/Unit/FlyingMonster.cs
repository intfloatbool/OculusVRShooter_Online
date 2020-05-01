using System.Collections;
using Photon.Pun;
using RealWorldVRGame.AI;
using UnityEngine;
using UnityEngine.AI;

namespace RealWorldVRGame
{
    public class FlyingMonster : OnlineEnemyAi
    {
        [Header("Flying unit references")] 
        [SerializeField] private bool _isFloating;
        [SerializeField] private float _floatingSpeed = 3f;
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _rotationSpeed = 6f;

        [SerializeField] private AudioClip _flappingSound;
        private Vector3? TargetFixedPosition
        {
            get
            {
                if (_currentTarget == null || _currentTarget.IsDead)
                    return null;
                var targetPosition = _currentTarget.transform.position;
                var targetPositionFixed = new Vector3(
                    targetPosition.x,
                    transform.position.y,
                    targetPosition.z
                );
                return targetPositionFixed;
            }
        }
        private bool IsTargetReachable
        {
            get
            {
                if (TargetFixedPosition == null)
                    return false;
                var distanceFromTarget = Vector3.Distance(TargetFixedPosition.Value, transform.position);
                var isTargetReachable = distanceFromTarget <= _attackRange;
                return isTargetReachable;
            }
        }
        
        protected override void Start()
        {
            Debug.Assert(_agent == null, "Agent should be null!");
            Debug.Assert(GetComponent<NavMeshAgent>() == null, "Remove navmesh from flying unit!");
            Debug.Assert(_unitStats != null, "_unitStats != null");
            Debug.Assert(_animator != null, "_animator != null");

            if (_flappingSound != null && _unitStats.AudioSource != null)
            {
                StartCoroutine(FlappingSoundPlayingCoroutine());
            }
        }

        private IEnumerator FlappingSoundPlayingCoroutine()
        {
            var waitingTime = new WaitForSeconds(_flappingSound.length);
            while (!_unitStats.IsDead)
            {
                _unitStats?.AudioSource?.PlayOneShot(_flappingSound);
                yield return waitingTime;
            }
        }

        public override void Initialize()
        {
            
            if (photonView.IsMine)
            {
                if (_isAutoSearchingForTarget)
                {
                    _targetSearchingRoutine = StartCoroutine(TargetSearchingRoutine());
                }

                _unitStats.OnDeath += OnDeathLocal;
                _unitStats.OnDamaged += OnDamagedLocal;

                _isInitialized = true;
            }
        }

        protected override void ControlMovingLoop()
        {
     
            if (!IsTargetReachable && _currentTarget != null)
            {
                LookAtTarget();   
                transform.Translate(Vector3.forward * _moveSpeed * Time.deltaTime);
            }
        }
        
        protected override void ControlAttackLoop()
        {
            //TODO: Remove agent dependency!
            if (IsTargetReachable && _currentTarget != null)
            {
                LookAtTarget();
                if (_attackTimer >= _attackDelay)
                {
                    LocalAttack();
                    _attackTimer = 0;
                }

                _attackTimer += Time.deltaTime;
            }
        }

        protected override void LookAtTarget()
        {
            if (TargetFixedPosition == null)
                return;
            var relativePos = TargetFixedPosition.Value - transform.position;
            var rotationToLook = Quaternion.LookRotation(relativePos, Vector3.up);
            transform.rotation =
                Quaternion.Lerp(transform.rotation, rotationToLook, _rotationSpeed * Time.deltaTime);
        }

        protected override void OnAttack()
        {
            base.OnAttack();
            Debug.Log("Flying unit attack!");
        }

        protected override void OnHit()
        {
            Stun();
        }

        protected override void Update()
        {
            if (_currentTarget == null || _currentTarget.IsDead)
                return;
            if (photonView.IsMine && _isInitialized)
            {
                ControlStunLoop();
                ControlMovingLoop();
                ControlAttackLoop();
            }

            ControlAnimationLoop();
            ControlDecayLoop();
        }
    }
}