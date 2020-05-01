using Photon.Pun;
using RealWorldVRGame.Enums;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RealWorldVRGame
{
    public class VrPlayer : MonoBehaviourPun
    {
        [SerializeField] private WeaponBase _currentWeapon;
        public WeaponBase CurrentWeapon => _currentWeapon;
        [SerializeField] private VrBody _vrBody;
        public void EquipGun(WeaponBase weapon)
        {
            _currentWeapon = weapon;
        }

        [SerializeField] private List<WeaponBase> _weapons;

        private void Start()
        {
            //Test equip
            if(photonView.IsMine)
            {
                photonView.RPC("EquipWeaponRPC", RpcTarget.AllBuffered, 0);
            }
        }

        [PunRPC]
        public void EquipWeaponRPC(int weapIndex)
        {
            var weaponType = (WeaponType)weapIndex;
            _currentWeapon = GetWeaponByType(weaponType);
            if (_currentWeapon != null)
            {
                _weapons.ForEach(w => w.gameObject.SetActive(w.WeaponType == weaponType));
            }
        }


        private WeaponBase GetWeaponByType(WeaponType weaponType)
        {         
            var weapon = _weapons.FirstOrDefault(w => w.WeaponType == weaponType);
            if(weapon == null)
            {
                Debug.LogError($"Cannot find weapon with type: {weaponType}!");
            }
            return weapon;
        }

        private void Update()
        {
            if(photonView.IsMine)
            {
                if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch)
                    || Input.GetKeyDown(KeyCode.Space))
                {
                    photonView.RPC("TryShotFromCurrentWeaponRPC", RpcTarget.All);
                }
            }

            HandleWeaponPosition();
        }

        [PunRPC]
        public void TryShotFromCurrentWeaponRPC()
        {
            if (_currentWeapon != null)
            {
                _currentWeapon.Shot();
            }
        }

        private void HandleWeaponPosition()
        {
            if(_currentWeapon != null)
            {
                _currentWeapon.transform.position = _vrBody.RightHandWirst.position;
                _currentWeapon.transform.rotation = _vrBody.RightHandWirst.rotation;
            }
        }
    }
}

