using System;
using UnityEngine;

namespace RealWorldVRGame
{
    public class UnitStats : MonoBehaviour
    {
        [SerializeField] protected int _currentHp = 100;

        [Space(5f)] 
        [Header("Debug")]
        [SerializeField] protected bool _isDebug = true;

        [SerializeField] protected KeyCode _keyToKill = KeyCode.K;
        
        public int CurrentHp
        {
            get { return _currentHp; }
            set { this._currentHp = value; }
        }
        protected int _basicHp;

        [SerializeField] protected bool _isDead;
        public bool IsDead => _isDead;

        [SerializeField] protected AudioClip _onHitSound;
        [SerializeField] protected AudioClip _onDeathSound;
        [SerializeField] protected AudioSource _audioSource;
        public AudioSource AudioSource => _audioSource;

        public event Action<int> OnDamaged = (damage) => { };
        public event Action OnDeath = () => { };

        protected void Start()
        {
            _basicHp = _currentHp;
        }
        
        public void MakeDamage(int damage)
        {
            OnDamaged(damage);
        }

        public virtual void Death()
        {
            _isDead = true;
            PlayDeathSound();
            OnDeath();
            
        }

        public void PlayDamageSound()
        {
            PlaySound(_onHitSound);
        }

        public void PlayDeathSound()
        {
            PlaySound(_onDeathSound);
        }

        public virtual void ResurrectUnit()
        {
            _isDead = false;
            _currentHp = _basicHp;

            
        }

        protected void PlaySound(AudioClip clip)
        {
            if (_audioSource != null && clip != null)
            {
                _audioSource.PlayOneShot(clip);
            }
        }

        protected void Update()
        {
            if (_isDebug)
            {
                if(Input.GetKeyDown(_keyToKill))
                    SelfKill();
            }
        }

        //DEBUG
        [ContextMenu("Selft damage 50")]
        public void SelfDamage()
        {
            MakeDamage(50);
        }
        
        [ContextMenu("Kill unit")]
        public void SelfKill()
        {
            MakeDamage(_currentHp);
        }
    }
}