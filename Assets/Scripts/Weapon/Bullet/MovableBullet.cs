using System.Collections;
using UnityEngine;

namespace RealWorldVRGame
{
    public class MovableBullet : MonoBehaviour
    {       
        [SerializeField] protected Transform _deathEffect;
        [SerializeField] protected Transform _bulletBody;
        [SerializeField] protected float _decayTime = 2.5f;
        [SerializeField] protected float _lifeTime = 10f;
        [SerializeField] protected bool _isActive;
        private float _currentLifeTimer;
        public bool IsActive
        {
            get { return this._isActive; }
            set { this._isActive = value; }
        }
        [SerializeField] protected float _speed = 8f;
        public float Speed => _speed;
        protected virtual void Update()
        {
            if (_isActive)
            {
                transform.Translate(Vector3.forward * _speed * Time.deltaTime);
                CalculateLifeTime();
            }
        }
        
        protected virtual void OnBang(UnitStats unitStats = null)
        {
            _isActive = false;
            if(_bulletBody != null)
                _bulletBody.gameObject.SetActive(false);
            if(_deathEffect != null)
                _deathEffect.gameObject.SetActive(true);
            _currentLifeTimer = 0;
            StartCoroutine(DecayCoroutine());
        }

        protected IEnumerator DecayCoroutine()
        {
            yield return new WaitForSeconds(_decayTime);
            gameObject.SetActive(false);
        }

        protected virtual void CalculateLifeTime()
        {
            if (_currentLifeTimer > _lifeTime)
            {
                OnBang();
            }
            _currentLifeTimer += Time.deltaTime;
        }

        public void ResetBullet()
        {
            if(_bulletBody != null)
                _bulletBody.gameObject.SetActive(true);
            if(_deathEffect != null)
                _deathEffect.gameObject.SetActive(false);
        }
        

    }
}
