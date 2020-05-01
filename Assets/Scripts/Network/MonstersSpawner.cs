using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using RealWorldVRGame.AI;
using UnityEngine;
using Random = UnityEngine.Random;


namespace RealWorldVRGame
{
    public class MonstersSpawner : MonoBehaviourPunCallbacks
    {
        [SerializeField] private string[] _mobsPrefabNames;
        [SerializeField] private bool _isMobsSpawningEnabled;
        [SerializeField] private float _spawnDelay = 10f;
        [SerializeField] private int _maxMobs = 3;
        [SerializeField] private List<OnlineEnemyAi> _currentMobs;
        [SerializeField] private Transform[] _mobsPositions;
        [SerializeField] private string _flyingMonstersTag = "FLY";
        [SerializeField] private float _flyingMonstersYPos = 4.5f;
        public override void OnJoinedRoom()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("YOU ARE MASTER CLIENT INITIALIZED!!!");
                if (_isMobsSpawningEnabled)
                {
                    if(_currentMobs == null)
                        _currentMobs = new List<OnlineEnemyAi>();
                    StartCoroutine(MobsSpawnCoroutine());
                }
            }
        }

        private IEnumerator MobsSpawnCoroutine()
        {
            while (true)
            {
                //Clear from dead mobs
                _currentMobs.RemoveAll(m => m == null || m.UnitStats.IsDead);
                
                yield return new WaitForSeconds(_spawnDelay);
                if (_currentMobs.Count <= _maxMobs - 1)
                {
                    var mob = CreateRandomMob();
                    if (mob != null)
                    {
                        mob.Initialize();
                        _currentMobs.Add(mob);
                    }  
                }
                
            }
        }

        private OnlineEnemyAi CreateRandomMob()
        {
            var rndName = _mobsPrefabNames[Random.Range(0, _mobsPrefabNames.Length)];
            Debug.Assert(string.IsNullOrEmpty(rndName) == false, "string.IsNullOrEmpty(rndName) == false");
            if (string.IsNullOrEmpty(rndName))
                return null;
            var rndPosition = _mobsPositions[Random.Range(0, _mobsPositions.Length)];
            var positionToInstantiate = rndPosition.position;
            if (rndName.Contains(_flyingMonstersTag))
            {
                positionToInstantiate = new Vector3(
                    rndPosition.position.x,
                    _flyingMonstersYPos,
                    rndPosition.position.z
                );
            }
            Debug.Assert(rndPosition != null, "rndPosition != null");
            if (rndPosition == null)
                return null;
            var mobGO = PhotonNetwork.Instantiate(rndName, positionToInstantiate, Quaternion.identity);
            Debug.Assert(mobGO != null, "mobGO != null");
            if (mobGO == null)
                return null;
            var aiMob = mobGO.GetComponent<OnlineEnemyAi>();
            Debug.Assert(aiMob != null, "aiMob != null");
            if (aiMob == null)
                return null;
            
            return aiMob;
        }
    }

}
