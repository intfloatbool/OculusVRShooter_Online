using System;
using UnityEngine;


namespace RealWorldVRGame
{
    public class DeathBody : MonoBehaviour
    {
        [SerializeField] private float _fallSpeed = 3f;
        [SerializeField] private float _angleX = -85;
        [SerializeField] private bool _isDeath;
        public void StartDeath()
        {
            _isDeath = true;
        }

        
        [ContextMenu("Reset body")]
        public void Reset()
        {
            _isDeath = false;
            transform.position = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;
        }

        public void ResetByOrigin(Transform origin)
        {
            _isDeath = false;
            transform.position = origin.position;
            transform.rotation = origin.rotation;
        }

        private void Update()
        {
            if (_isDeath)
            {
                var fallEuler = new Vector3(_angleX, 0, 0);
                if (Quaternion.Angle(transform.rotation,
                        Quaternion.Euler(fallEuler)) > 1f)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(fallEuler), 
                        _fallSpeed * Time.deltaTime);
                }
            }
        }
    }
}

