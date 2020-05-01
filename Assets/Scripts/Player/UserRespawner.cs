using System;
using Passer;
using RealWorldVRGame.UI;
using UnityEngine;

namespace RealWorldVRGame
{
    public class UserRespawner : MonoBehaviour
    {
        [SerializeField] private float _minDistance = 0.05f;
        [SerializeField] private float _minAngle = 1f;
        [SerializeField] private ProgressImg _resurrectionProgressImage;
        [SerializeField] private float _resurrectionTime = 3f;
        private float _currentTimer;
        public event Action<UnitStats> OnPlayerRespawnReady = (unit) => { };
        [Header("Runtime references")]
        [SerializeField] private PlayerUnitStats _localUnit;
        public PlayerUnitStats LocalUnit
        {
            get => _localUnit;
            set => _localUnit = value;
        }

        [SerializeField] private PlayerUnitStats _currentUnit;
        

        private void Start()
        {
            Debug.Assert(_resurrectionProgressImage != null, "_resurrectionProgressImage != null");
        }

        private void OnTriggerEnter(Collider other)
        {
            var unit = other.GetComponent<PlayerUnitStats>();
            if (unit != _localUnit)
                return;
            _currentUnit = unit;
            if(_currentUnit == null)
                return;
            if (!_currentUnit.IsDead) 
                return;
            if (_resurrectionProgressImage != null)
            {
                _resurrectionProgressImage.gameObject.SetActive(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_currentUnit != null && !_currentUnit.IsDead)
            {
                _currentUnit = null;
            }
            
        }

        private void Update()
        {
            if(_currentUnit == null || !_currentUnit.IsDead)
                return;

            var currentUnitPos = _currentUnit.transform.position;
            var optimizedUnitPos = new Vector3(
                currentUnitPos.x,
                transform.position.y,
                currentUnitPos.z
                );
            var currentUnitRotation = _currentUnit.transform.rotation;
            var currentDistance = Vector3.Distance(optimizedUnitPos, transform.position);
            var currentAngle = Quaternion.Angle(currentUnitRotation, transform.rotation);
            var normalizedTime = _currentTimer / _resurrectionTime;
            if (currentDistance <= _minDistance && currentAngle <= _minAngle)
            {
                _currentTimer += Time.deltaTime;
                if (_currentTimer >= _resurrectionTime)
                {
                    OnPlayerRespawnReady(_currentUnit);
                    _currentTimer = 0;
                    if (_resurrectionProgressImage != null)
                    {
                        _resurrectionProgressImage.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                _currentTimer = 0;
            }
            
            if (_resurrectionProgressImage != null)
            {
                _resurrectionProgressImage.CurrentValue = normalizedTime;
            }
        }
    }

}
