using RealWorldVRGame.Enums;
using UnityEngine;

namespace RealWorldVRGame
{
    public abstract class WeaponBase : MonoBehaviour
    {
        [Header("ON shot sound")]
        [SerializeField] protected AudioSource _audioSource;
        [Space(5f)]
        [SerializeField] protected WeaponType _weaponType;
        public WeaponType WeaponType => _weaponType;

        [Space(5f)]
        [SerializeField] protected float _reloadDelay = 3f;
        [SerializeField] protected bool _isReady;
        protected float _currentTimer;
        public virtual void Shot()
        {
            if(_isReady)
            {
                OnShot();
                _isReady = false;
                PlaySound();
            }
        }

        protected virtual void Update()
        {
            if(!_isReady)
            {
                HandleTimer();
            }
        }

        protected virtual void HandleTimer()
        {
            if (_currentTimer > _reloadDelay)
            {
                _currentTimer = 0;
                _isReady = true;
            }

            _currentTimer += Time.deltaTime;
        }

        protected abstract void OnShot();

        protected virtual void PlaySound()
        {
            if(_audioSource != null)
                _audioSource.Play();
        }
    }
}

