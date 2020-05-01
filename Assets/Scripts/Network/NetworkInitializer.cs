using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkTesting
{
    public class NetworkInitializer : MonoBehaviourPunCallbacks
    {
        [Header("if == -1 , is infinity")]
        [SerializeField] private int _maxPlayers = 8;
        [SerializeField] private string _testRoomName = "testRoom";
        [SerializeField] private GameObject _networkPlayerPrefab;
        [SerializeField] private Transform[] _spawnPositions;
        [SerializeField] private Vector3 _spawnOffset = Vector3.zero;
        private void Start()
        {
            PhotonNetwork.NickName = $"Player{Random.Range(0, 5555)}";
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.GameVersion = "1";
            PhotonNetwork.ConnectUsingSettings();      
        }

        public override void OnConnectedToMaster()
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.IsVisible = true;
            if(_maxPlayers > 1)
            {
                if(_maxPlayers >= byte.MaxValue)
                {
                    _maxPlayers = byte.MaxValue - 1;
                }
                roomOptions.MaxPlayers = (byte) _maxPlayers;
            }
            
            PhotonNetwork.JoinOrCreateRoom(_testRoomName, roomOptions, TypedLobby.Default);        
        }

        private void CreatePlayer()
        {
            var rndPos = _spawnPositions[Random.Range(0, _spawnPositions.Length)].position;
            var posToInstantiate = rndPos + _spawnOffset;
            var player = PhotonNetwork.Instantiate(_networkPlayerPrefab.name, posToInstantiate, Quaternion.identity, 0);
            player.name = PhotonNetwork.NickName;
        }


        public override void OnJoinedRoom()
        {
            Debug.Log($"Join room: {PhotonNetwork.CurrentRoom.Name}, \n Player: {PhotonNetwork.NickName}!");
            CreatePlayer();
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.LogError($"Join room failed! \n code: {returnCode} \n msg: {message}");
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.Log($"Player {newPlayer.NickName} entred in room");
        }

        public override void OnCustomAuthenticationFailed(string debugMessage)
        {
            Debug.LogError("OnCustomAuthenticationFailed: " + debugMessage);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.Log($"Player {otherPlayer.NickName} leaved from room");
        }
    }
}
