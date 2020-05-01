using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RealWorldVRGame
{
    public class VrPlayerNetworkInitializer : MonoBehaviourPun, IPunObservable
    {

        [SerializeField] private GameObject _vrRig;
        [SerializeField] private Transform _centerVrEye;
        [SerializeField] private Transform _leftVrHand;
        [SerializeField] private Transform _rightVrHand;

        [SerializeField] private VrBody _vrBody;
        [SerializeField] private Transform _debugUI;
        //Body positions
        private Vector3 _currentBodyPosition;
        private Quaternion _currentBodyRotation;

        private Vector3 _currentRightHandPosition;
        private Quaternion _currentRightHandRotation;

        private Vector3 _currentLeftHandPosition;
        private Quaternion _currentLeftHandRotation;

        private Quaternion _currentHeadRotation;

        private void Start()
        {
            _currentBodyPosition = _vrBody.PlayerBody.position;
            _currentBodyRotation = _vrBody.PlayerBody.rotation;

            _currentRightHandPosition = _vrBody.RightHandWirst.position;
            _currentRightHandRotation = _vrBody.RightHandWirst.rotation;

            _currentLeftHandPosition = _vrBody.LeftHandWirst.position;
            _currentLeftHandRotation = _vrBody.LeftHandWirst.rotation;

            _currentHeadRotation = _vrBody.PlayerHead.rotation;

            if (photonView.IsMine)
            {
                _vrRig.SetActive(true);
                _vrBody.PlayerHead.gameObject.SetActive(false);
                _debugUI.gameObject.SetActive(true);
            }
        }

        private void Update()
        {
            if(photonView.IsMine)
            {
                _currentBodyPosition = _centerVrEye.position;
                _currentBodyRotation = _centerVrEye.rotation;

                _currentLeftHandPosition = _leftVrHand.position;
                _currentLeftHandRotation = _leftVrHand.rotation;

                _currentRightHandPosition = _rightVrHand.position;
                _currentRightHandRotation = _rightVrHand.rotation;

                _currentHeadRotation = _centerVrEye.rotation;

                _vrBody.TrackBodyForAnotherTransformLoop(
                    _currentBodyPosition,
                    _currentBodyRotation
                    );

                _vrBody.TrackLeftHandForAnotherTransformLoop(
                    _currentLeftHandPosition,
                    _currentLeftHandRotation
                    );

                _vrBody.TrackRightHandForAnotherTransformLoop(
                    _currentRightHandPosition,
                    _currentRightHandRotation
                    );

                _vrBody.TrackHeadForAnotherTransformLoop(_currentHeadRotation);
            }
            else
            {
                _vrBody.TrackBodyForAnotherTransformLoop(
                    _currentBodyPosition, 
                    _currentBodyRotation, 
                    true);

                _vrBody.TrackLeftHandForAnotherTransformLoop(
                    _currentLeftHandPosition,
                    _currentLeftHandRotation,
                    true);

                _vrBody.TrackRightHandForAnotherTransformLoop(
                    _currentRightHandPosition,
                    _currentRightHandRotation,
                    true);

                _vrBody.TrackHeadForAnotherTransformLoop(_currentHeadRotation);
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(this._currentBodyPosition);
                stream.SendNext(this._currentBodyRotation);

                stream.SendNext(this._currentLeftHandPosition);
                stream.SendNext(this._currentLeftHandRotation);

                stream.SendNext(this._currentRightHandPosition);
                stream.SendNext(this._currentRightHandRotation);

                stream.SendNext(this._currentHeadRotation);
            }
            else
            {
                this._currentBodyPosition = (Vector3)stream.ReceiveNext();
                this._currentBodyRotation = (Quaternion)stream.ReceiveNext();

                this._currentLeftHandPosition = (Vector3)stream.ReceiveNext();
                this._currentLeftHandRotation = (Quaternion)stream.ReceiveNext();

                this._currentRightHandPosition = (Vector3)stream.ReceiveNext();
                this._currentRightHandRotation = (Quaternion)stream.ReceiveNext();

                this._currentHeadRotation = (Quaternion)stream.ReceiveNext();
            }
        }
    }

}