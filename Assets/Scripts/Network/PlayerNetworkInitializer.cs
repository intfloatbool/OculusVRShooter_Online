using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using System.Linq;
using System;

namespace NetworkTesting
{
    public class PlayerNetworkInitializer : MonoBehaviourPun, IPunObservable
    {

        private Vector3 _currentPosition;
        private Quaternion _currentRotation;

        [SerializeField] private Transform _playerBodyTransform;
        [SerializeField] private Transform _vrBodyAnchor;

        [SerializeField] private OVRPlayerController _playerController;
        [SerializeField] private Transform _cameraRigContainer;
        [SerializeField] private float _smoothSpeed = 7;

        public event Action OnPlayerInitialized = () => { };
        
        private void Start()
        {
            _currentPosition = transform.position;
            _currentRotation = transform.rotation;

            Debug.Log("IS mine? :" + photonView.IsMine);
            SetActiveComponents(photonView.IsMine);        
        }

        private void Update()
        {
            if(photonView.IsMine)
            {
                _currentPosition = _vrBodyAnchor.position;
                _currentRotation = _vrBodyAnchor.rotation;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, _currentPosition, _smoothSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Lerp(transform.rotation, _currentRotation, _smoothSpeed * Time.deltaTime);
            }
            
        }
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if(stream.IsWriting)
            {
                stream.SendNext(this._currentPosition);
                stream.SendNext(this._currentRotation);
            }
            else
            {
                this._currentPosition = (Vector3) stream.ReceiveNext();
                this._currentRotation = (Quaternion) stream.ReceiveNext();
            }
        }

        private void SetActiveComponents(bool isActive)
        {
            if(isActive)
            {
                _cameraRigContainer.gameObject.SetActive(true);
                _playerBodyTransform.parent = _vrBodyAnchor;
                _playerController.InitializeController();
                _playerController.InitializeRig();
            }
            else
            {
                Destroy(_playerController);
                Destroy(_cameraRigContainer.gameObject);
            }

            OnPlayerInitialized();
        }
    }

}
