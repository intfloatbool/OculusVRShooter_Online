using UnityEngine;

namespace RealWorldVRGame
{   
    public class PlayerAnimator : MonoBehaviour
    {
        [SerializeField] private string _walkingAnimationParam = "IsWalking";
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _trackingBody;
        [SerializeField] private float _minLengthToActive = 0.15f;
        [SerializeField] private float _timeToUpdatePosition = 2f;
        private float _currentTimer = 0;
        private Vector3 _lastPosition;
        private Vector3 _currentPosition;

        private void Start()
        {
            _lastPosition = _trackingBody.position;
            _currentPosition = _trackingBody.position;
        }

        private void Update()
        {
            _currentPosition = _trackingBody.position;

            var relativePos = _currentPosition - _lastPosition;
            var magnitude = relativePos.magnitude;
            _animator.SetBool(_walkingAnimationParam, magnitude > _minLengthToActive);

            if(_currentTimer > _timeToUpdatePosition)
            {
                _currentTimer = 0;
                _lastPosition = _trackingBody.position;
            }

            _currentTimer += Time.deltaTime;
        }

    }
}

