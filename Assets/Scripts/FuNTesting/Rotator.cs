using System;
using System.Collections;
using UnityEngine;
using Random = System.Random;

namespace FunTesting
{
    public class Rotator : MonoBehaviour
    {
        [SerializeField] private float _rotMinSpeed = 6f;
        [SerializeField] private float _rotMaxSpeed = 35f;
        private float _currentSpeed = 10;
        [SerializeField] private Vector3[] _rndDicrections;
        private Vector3 _currentDirection;
        [SerializeField] private float _directionChangeTime = 3f;
        [SerializeField] private float _delayBetweenStart = 6f;
        [SerializeField] private bool _isActive;
        private void Awake()
        {
            _rndDicrections = new[]
            {
                new Vector3(1, 0, 0), 
                new Vector3(0, 1, 0), 
                new Vector3(0, 0, 1),
                new Vector3(-1, 0, 0), 
                new Vector3(0, -1, 0), 
                new Vector3(0, 0, -1)
            };
        }

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(_delayBetweenStart);
            _isActive = true;
            while (_isActive)
            {
                yield return new WaitForSeconds(_directionChangeTime);
                _currentDirection = _rndDicrections[UnityEngine.Random.Range(0, _rndDicrections.Length)];
                _currentSpeed = UnityEngine.Random.Range(_rotMinSpeed, _rotMaxSpeed);
            }
        }

        private void Update()
        {
            if(!_isActive)
                return;
            transform.Rotate(_currentDirection, _currentSpeed * Time.deltaTime);
            transform.Translate(transform.up * _currentSpeed * Mathf.Sin(Time.time) * Time.deltaTime, Space.Self);
        }
    }
}

