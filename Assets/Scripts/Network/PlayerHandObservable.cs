using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkTesting
{
    public class PlayerHandObservable : MonoBehaviourPun, IPunObservable
    {
        [System.Serializable]
        public struct HandVirtualTransform
        {
            public Vector3 WorldPosition;
            public Quaternion WorldRotation;

            public void SetTransform(Vector3 pos, Quaternion rot)
            {
                WorldPosition = pos;
                WorldRotation = rot;
            }
        }

        [SerializeField] private PlayerNetworkInitializer _networkInitializer;
        [SerializeField] private float _smoothSpeed;

        [SerializeField] private Transform _handsContainer;
        [SerializeField] private Transform _rightHand;
        [SerializeField] private Transform _leftHand;

        [SerializeField] private HandVirtualTransform _currentRightHandTransform;
        [SerializeField] private HandVirtualTransform _currentLeftHandTransform;

        private OvrAvatar _ovrAvatar;

        private void Awake()
        {
            _networkInitializer.OnPlayerInitialized += StartInitHands;

            //basic
            _currentLeftHandTransform.SetTransform(_leftHand.position, _leftHand.rotation);
            _currentRightHandTransform.SetTransform(_rightHand.position, _rightHand.rotation);
        }

        private void StartInitHands()
        {
            _ovrAvatar = GetComponentInChildren<OvrAvatar>(false);
            //check local or remote player
            var isLocalPlayer = _ovrAvatar == null;
            _handsContainer.gameObject.SetActive(isLocalPlayer);
        } 

        private void Update()
        {

            if (photonView.IsMine)
            {
                if(_ovrAvatar != null)
                {
                    var avatarLeftHand =
                       _ovrAvatar.GetHandTransform(OvrAvatar.HandType.Left, OvrAvatar.HandJoint.HandBase);
                    var avatarRightHand = 
                        _ovrAvatar.GetHandTransform(OvrAvatar.HandType.Right, OvrAvatar.HandJoint.HandBase);

                    if(avatarLeftHand != null)
                    {
                        _currentLeftHandTransform.WorldPosition = avatarLeftHand.position;
                        _currentLeftHandTransform.WorldRotation = avatarLeftHand.rotation;
                    }

                    if (avatarRightHand != null)
                    {
                        _currentRightHandTransform.WorldPosition = avatarRightHand.position;
                        _currentRightHandTransform.WorldRotation = avatarRightHand.rotation;
                    }
                }
              
            }
            else
            {
                SetTransformOfHand(_leftHand, _currentLeftHandTransform);
                SetTransformOfHand(_rightHand, _currentRightHandTransform);
            }
        }

        private void SetTransformOfHand(Transform hand, HandVirtualTransform virtualTransform)
        {
            if (hand != null)
            {
                hand.position = Vector3.Lerp(hand.position,
                    virtualTransform.WorldPosition,
                    _smoothSpeed * Time.deltaTime);
                hand.rotation = Quaternion.Lerp(hand.rotation,
                    virtualTransform.WorldRotation,
                    _smoothSpeed * Time.deltaTime);
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(this._currentLeftHandTransform.WorldPosition);
                stream.SendNext(this._currentLeftHandTransform.WorldRotation);

                stream.SendNext(this._currentRightHandTransform.WorldPosition);
                stream.SendNext(this._currentRightHandTransform.WorldRotation);
            }
            else
            {
                _currentLeftHandTransform.WorldPosition = (Vector3)stream.ReceiveNext();
                _currentLeftHandTransform.WorldRotation = (Quaternion) stream.ReceiveNext();

                _currentRightHandTransform.WorldPosition = (Vector3)stream.ReceiveNext();
                _currentRightHandTransform.WorldRotation = (Quaternion)stream.ReceiveNext();
            }
        }
    }
}
