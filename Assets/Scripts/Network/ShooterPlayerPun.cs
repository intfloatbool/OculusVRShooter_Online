using System;
using System.Collections;
using System.Collections.Generic;
using Passer;
using Photon.Pun;
using RealWorldVRGame.UI;
using UnityEngine;

namespace RealWorldVRGame
{
    public class ShooterPlayerPun : MonoBehaviourPun
    {
        [SerializeField] private HumanoidPun _humanoidPun;
        
        [Header("Runtime references")]
        [SerializeField] private HumanoidControl _humanoidControl;
        [SerializeField] private PlayerUnitStats _unitStats;

        private bool _isInitialized = false;
        
        private void Awake()
        {
            Debug.Assert(_humanoidPun != null, "_humanoidPun != null");
            _humanoidPun.OnPlayerInitialized += OnPlayerInitialized;
        }

        private void OnPlayerInitialized(HumanoidControl humanoidControl, UnitStats unitStats)
        {
            _humanoidControl = humanoidControl;
            Debug.Assert(_humanoidControl != null, "_humanoidControl != null");
            
            _unitStats = (PlayerUnitStats) unitStats;
            Debug.Assert(_unitStats != null, "_unitStats != null");

            if (photonView.IsMine)
            {
                _unitStats.OnDamaged += OnDamaged;
                var respawner = FindObjectOfType<UserRespawner>();
                if (respawner != null)
                {
                    respawner.LocalUnit = _unitStats;
                    respawner.OnPlayerRespawnReady += OnPlayerRespawnReady;
                }
                Debug.Assert(respawner != null, "respawner != null");
                _isInitialized = true;
            }
        }

        private void OnPlayerRespawnReady(UnitStats unitStats)
        {
            Debug.Assert(unitStats != null, "unitStats != null");
            Debug.Assert(unitStats == _unitStats, "unitStats == local _unitStats");
            
            _humanoidPun.ResurrectionLocal();
        }
        
        private void OnDamaged(int dmg)
        {
            photonView.RPC("OnDamageRPC", RpcTarget.All, dmg);
        }
        

        [PunRPC]
        public void OnDamageRPC(int dmg)
        {
            if (_unitStats == null)
                return;
            if (_unitStats.IsDead)
                return;
            _unitStats.CurrentHp -= dmg;
            _unitStats.PlayDamageSound();
            if (_unitStats.CurrentHp <= 0)
            {
                _unitStats.CurrentHp = 0;
                _unitStats.Death();
            }
        }

        private void Update()
        {
            if (!_isInitialized)
                return;
            
            if (photonView.IsMine)
            {
                //local logic
            }
        }
    }
}

