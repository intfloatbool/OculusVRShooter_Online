using System;
using Photon.Pun;
using UnityEngine;

namespace RealWorldVRGame
{
    public class GrenadeWeapon : WeaponBase, IPunWeapon
    {
        [SerializeField] private Grenade _grenadePrefab;
        [SerializeField] private GameObject _grenadeMainBody;
        [SerializeField] private GameObject _grenadeInactiveBody;
        [SerializeField] private float _throwPower = 400f;
        public bool IsMine { get; set; }
        private void Start()
        {
            Debug.Assert(_grenadeMainBody != null, "_grenadeMainBody != null");
            Debug.Assert(_grenadeInactiveBody != null, "_grenadeInactiveBody != null");
            Debug.Assert(_grenadePrefab != null, "_grenadePrefab != null");
        }

        protected override void Update()
        {
            base.Update();

            if (_grenadeMainBody != null && _grenadeInactiveBody != null)
            {
                _grenadeMainBody.SetActive(_isReady);
                _grenadeInactiveBody.SetActive(!_isReady);
            }
        }
        
        protected override void OnShot()
        {
            //TODO: realize grenade logic
            if (IsMine)
            {
                var granadeGO = PhotonNetwork.Instantiate(_grenadePrefab.name, transform.position, Quaternion.identity);
                var granadeRB = granadeGO.GetComponent<Rigidbody>();
                Debug.Assert(granadeGO != null, "granadeGO != null");
                Debug.Assert(granadeRB != null, "granadeRB != null");
                if (granadeRB != null)
                {
                    granadeRB.AddForce(-transform.right * _throwPower);
                }
            }
        }
    }
}