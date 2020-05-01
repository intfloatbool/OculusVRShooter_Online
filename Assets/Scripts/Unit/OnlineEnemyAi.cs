using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace RealWorldVRGame.AI
{
    public class OnlineEnemyAi : MonoBehaviourPun
    {
        [SerializeField] protected NavMeshAgent _agent;
        [SerializeField] protected UnitStats _unitStats;
        public UnitStats UnitStats => _unitStats;

        [Header("Animations")] [SerializeField]
        protected Animator _animator;

        [SerializeField] protected string[] _attackTriggers;
        [SerializeField] protected string[] _hitTriggers;
        [SerializeField] protected string _deathTrigger;
        [SerializeField] protected string _walkBoolName;

        [Space(5f)] [Header("Audio")] [SerializeField]
        private AudioSource _audioSource;

        [SerializeField] private AudioClip _attackSound;

        [Space(5f)] [Header("Behaviour variables")] [SerializeField]
        protected bool _isAutoSearchingForTarget = true;

        [SerializeField] protected float _targetSearchingDelay = 3f;
        [SerializeField] protected float _stunTime = 1.5f;
        [SerializeField] protected Vector2Int _damageRange;
        [SerializeField] protected float _attackRange = 1f;
        [SerializeField] protected float _attackDelay = 2f;
        [SerializeField] protected float _decayDelay = 3f;
        protected float _currentDecayTimer;
        protected bool _isDecayed;
        protected float _attackTimer;
        protected int RandomDamage => Random.Range(_damageRange.x, _damageRange.y);
        protected float _currentStunTimer;


        protected Coroutine _targetSearchingRoutine;

        [Space(5f)] [Header("Runtime local references")] [SerializeField]
        protected UnitStats _currentTarget;
        public Vector3 CurrentTargetPositionRPC { get; protected set; }
        [SerializeField] protected bool _isStunned;

        [Space(5f)] [Header("Runtime references")] [SerializeField]
        protected bool _isMoving;

        [SerializeField] protected bool _isInitialized;

        public event Action<UnitStats> OnUnitAttack = (target) => {
        };

        protected virtual void Start()
        {
            Debug.Assert(_agent != null, "_agent != null");
            Debug.Assert(_unitStats != null, "_unitStats != null");
            Debug.Assert(_animator != null, "_animator != null");
        }

        public virtual void Initialize()
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
            else
            {
                _agent.enabled = false;
            }
        }


        private UnitStats SearchRandomTarget()
        {
            var targets = FindObjectsOfType<PlayerUnitStats>();
            if (targets?.Length > 0)
            {
               return targets[Random.Range(0, targets.Length)];
            }

            return null;
        }

        protected IEnumerator TargetSearchingRoutine()
        {
            var waitingRef = new WaitForSeconds(_targetSearchingDelay);
            while (true)
            {
                while (_currentTarget == null || _currentTarget.IsDead)
                {
                    _currentTarget = SearchRandomTarget();
                    yield return waitingRef;
                }
                yield return waitingRef;
            }
        }

        protected void OnDamagedLocal(int dmg)
        {
            photonView.RPC("OnDamagedRPC", RpcTarget.All, dmg);
        }

        [PunRPC]
        public void OnDamagedRPC(int dmg)
        {
            OnDamaged(dmg);
        }

        protected virtual void OnDamaged(int dmg)
        {
            if(_unitStats == null)
                return;
            if (_unitStats.IsDead)
                return;
            _unitStats.CurrentHp -= dmg;
            _unitStats.PlayDamageSound();
            if (_unitStats.CurrentHp <= 0)
            {
                _unitStats.CurrentHp = 0;
                _unitStats.Death();
            }
            else
            {
                OnHit();
            }
        }
        
        [PunRPC]
        public void AttackRPC(Vector3 targetPosition)
        {
            CurrentTargetPositionRPC = targetPosition;
            OnAttack();
        }
        
        [ContextMenu("Attack debug")]
        public void LocalAttack()
        {
            var targetPosition = _currentTarget != null ? _currentTarget.transform.position : Vector3.zero;
            photonView.RPC("AttackRPC", RpcTarget.All, targetPosition);
        }

        protected virtual void OnAttack()
        {
            RunRandomAnimTrigger(_attackTriggers);
            PlayAttackSound();
            OnUnitAttack(_currentTarget);
        }

        protected virtual void RunRandomAnimTrigger(string[] triggerNames)
        {
            _animator.SetTrigger(triggerNames[Random.Range(0, triggerNames.Length)]);
        }

        protected virtual void Update()
        {
            if (photonView.IsMine && _isInitialized)
            {
                ControlStunLoop();
                ControlMovingLoop();
                ControlAttackLoop();
            }

            ControlAnimationLoop();
            ControlDecayLoop();
        }

        protected virtual void ControlAnimationLoop()
        {
            _animator.SetBool(_walkBoolName, _isMoving);
        }

        protected virtual void ControlDecayLoop()
        {
            if (_isDecayed)
            {
                _currentDecayTimer += Time.deltaTime;
                if (_currentDecayTimer >= _decayDelay)
                {
                    Destroy(gameObject);
                }
            }
        }

        protected virtual void ControlMovingLoop()
        {
            var isTargetReachable =_currentTarget != null && !_currentTarget.IsDead;
            if (isTargetReachable)
            {
                _agent.destination = _currentTarget.transform.position;
            }
            var isNotInAttackRange = _agent.remainingDistance > _attackRange;
            if (isTargetReachable && !_isStunned && isNotInAttackRange)
            {
                _agent.SetDestination(_currentTarget.transform.position);
                photonView.RPC("SetMovingRPC", RpcTarget.All, true);
            }
        }

        protected virtual void ControlAttackLoop()
        {
            var isTargetReachable =_currentTarget != null && !_currentTarget.IsDead;
            var isNotInAttackRange = _agent.remainingDistance > _attackRange;
            if (isTargetReachable && isNotInAttackRange == false)
            {
                photonView.RPC("SetMovingRPC", RpcTarget.All, false);
                LookAtTarget();
                if (_attackTimer >= _attackDelay)
                {
                    LocalAttack();
                    _attackTimer = 0;
                }

                _attackTimer += Time.deltaTime;
            }
        }

        protected virtual void LookAtTarget()
        {
            if (_currentTarget != null)
            {
                var targetPos = _currentTarget.transform.position;
                var optimizedPos = new Vector3()
                {
                    x = targetPos.x,
                    y = transform.position.y,
                    z = targetPos.z
                };

                var relativePos = optimizedPos - transform.position;
                transform.rotation = Quaternion.LookRotation(relativePos, Vector3.up);
            }
        }

        [PunRPC]
        public void SetMovingRPC(bool isMoving)
        {
            _isMoving = isMoving;
        }

        protected virtual void ControlStunLoop()
        {
            if (_isStunned)
            {
                _currentStunTimer += Time.deltaTime;
                if (_currentStunTimer >= _stunTime)
                {
                    _currentStunTimer = 0;
                    _isStunned = false;
                }
            }
        }

        protected virtual void OnHit()
        {
            Stun();
            RunRandomAnimTrigger(_hitTriggers);
        }
        

        protected virtual void Stun()
        {
            _currentStunTimer = 0;
            _isStunned = true;
        }

        protected void OnDeathLocal()
        {
            photonView.RPC("DeathRPC", RpcTarget.All);
        }

        [PunRPC]
        public void DeathRPC()
        {
            OnDeath();
        }

        protected virtual void OnDeath()
        {
            _animator.SetTrigger(_deathTrigger);
            _isDecayed = true;
        }

        protected virtual void PlayAttackSound()
        {
            PlaySound(_attackSound);
        }
        protected virtual void PlaySound(AudioClip clip)
        {
            if (_audioSource != null && clip != null)
            {
                _audioSource.PlayOneShot(clip);
            }
        }
        
    }
}

