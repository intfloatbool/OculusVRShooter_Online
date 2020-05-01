using System;
using UnityEngine;

namespace RealWorldVRGame.Environment
{
    public class FloatingTransform : MonoBehaviour
    {
        [SerializeField] private Vector3 _direction;
        [SerializeField] private float _speed = 4f;

        private void Update()
        {
            transform.Translate(_direction * _speed * Mathf.Sin(Time.time) * Time.deltaTime);
        }
    }
}

