using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RealWorldVRGame
{
    public class VrBody : MonoBehaviour
    {
        [SerializeField] private float _lerpSpeed = 7f;

        [Header("Left VR hand")]
        [SerializeField] private Transform _leftHandWirst;
        public Transform LeftHandWirst => _leftHandWirst;

        [SerializeField] private Transform _rightHandWirst;
        public Transform RightHandWirst => _rightHandWirst;

        [Space(2f)]
        [SerializeField] private Transform _playerBody;
        public Transform PlayerBody => _playerBody;

        [SerializeField] private Transform _playerHead;
        public Transform PlayerHead => _playerHead;

        [SerializeField] private Transform _leftFoot;
        public Transform LeftFoot => _leftFoot;

        [SerializeField] private Transform _rightFoot;
        public Transform RightFoot => _rightFoot;

        public void TrackHeadForAnotherTransformLoop(Quaternion rotation, bool lerped = false)
        {
            TrackRotationForAnotherTransformLoop(_playerHead, rotation, lerped);
        }

        public void TrackBodyForAnotherTransformLoop(Vector3 position, Quaternion rotation, bool lerped = false)
        {
            TrackPartForAnotherTransformLoop(_playerBody, position, rotation, lerped);

            //lock euler for body
            var currentEuler = _playerBody.transform.eulerAngles;
            _playerBody.transform.eulerAngles = new Vector3
            {
                x = 0,
                y = currentEuler.y,
                z = 0
            };
        }

        public void TrackRightHandForAnotherTransformLoop(Vector3 position, Quaternion rotation, bool lerped = false)
        {
            TrackPartForAnotherTransformLoop(_rightHandWirst, position, rotation, lerped);
        }


        public void TrackLeftHandForAnotherTransformLoop(Vector3 position, Quaternion rotation, bool lerped = false)
        {
            TrackPartForAnotherTransformLoop(_leftHandWirst, position, rotation, lerped);
        }

        private void TrackPartForAnotherTransformLoop(Transform bodyPart, Vector3 position, Quaternion rotation, bool lerped = false)
        {
            if (bodyPart == null)
                return;
            TrackPositionForAnotherTransformLoop(bodyPart, position, lerped);
            TrackRotationForAnotherTransformLoop(bodyPart, rotation, lerped);
        }

        private void TrackPositionForAnotherTransformLoop(Transform bodyPart, Vector3 position, bool lerped = false)
        {
            if (bodyPart == null)
                return;

            if (lerped)
            {
                bodyPart.position = Vector3.Lerp(bodyPart.position, position, _lerpSpeed * Time.deltaTime);
            }
            else
            {
                bodyPart.position = position;
            }
        }

        private void TrackRotationForAnotherTransformLoop(Transform bodyPart, Quaternion rotation, bool lerped = false)
        {
            if (bodyPart == null)
                return;

            if (lerped)
            {
                bodyPart.rotation = Quaternion.Lerp(bodyPart.rotation, rotation, _lerpSpeed * Time.deltaTime);
            }
            else
            {
                bodyPart.rotation = rotation;
            }
        }
    }
}

