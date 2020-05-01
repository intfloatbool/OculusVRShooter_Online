using System;
using System.Collections.Generic;
using System.Linq;
using RealWorldVRGame;
using RealWorldVRGame.Enums;
using UnityEngine;
using Random = System.Random;
#if hPHOTON2
using Photon.Realtime;
using Photon.Pun;
#endif

namespace Passer {

#if !hNW_PHOTON
    public class HumanoidPun : MonoBehaviour {
#else
    [RequireComponent(typeof(PhotonView))]
#if hPHOTON2
    public class HumanoidPun : MonoBehaviourPunCallbacks, IHumanoidNetworking, IPunInstantiateMagicCallback, IPunObservable {
#else
    public class HumanoidPun : Photon.MonoBehaviour, IHumanoidNetworking {
#endif
        private readonly string _defaultRightHandTransformName = "Default_simple|Hand_R";
        [SerializeField] private Transform _rightHandTransform;
        [SerializeField] private WeaponBase[] _weaponPrefabs;
        [SerializeField] private WeaponType _currentWeaponType;
        private int CurrentWeaponIndex => (int)_currentWeaponType;
        [SerializeField] private WeaponBase _currentWeapon;
        [SerializeField] private List<WeaponBase> _equipedWeapons;
        
        [SerializeField] private Color[] _playerColors;
        private int _currentColorIndex;
        private int RandomColorIndex => UnityEngine.Random.Range(0, _playerColors.Length);
        public bool syncFingerSwing = false;

        public HumanoidNetworking.Debug debug = HumanoidNetworking.Debug.Error;

        public bool isLocal = false;
        public bool IsLocal() { return isLocal; }
        public List<HumanoidControl> humanoids = new List<HumanoidControl>();
        private string PlayerID => $"{gameObject.name}_{photonView.ViewID}";
        
        [Header("Runtime references")]
        [SerializeField] private DeathController _deathController;
        [SerializeField] private HumanoidControl _playerHumanoidControl;
        [SerializeField] private UnitStats _playerUnitStats;
        [SerializeField] private bool _isCanEquipWeapon = true;

        public event Action<HumanoidControl, UnitStats> OnPlayerInitialized = (humanoid, unitStats) => { };
        
        private WeaponBase CreateWeaponByType(WeaponType weaponType)
        {
            var weaponPrefab = _weaponPrefabs.FirstOrDefault(wp => wp.WeaponType == weaponType);
            if (weaponPrefab != null)
            {
                if (_rightHandTransform == null)
                {
                    Debug.LogError("Cannot create weapon! There is no rightHand transform!");
                    return null;
                }
                return Instantiate(weaponPrefab, _rightHandTransform);
            }

            return null;
        }

        [PunRPC]
        public void SwitchWeaponRPC(int weaponType)
        {
            SwitchWeapon((WeaponType) weaponType);
        }
        private void SwitchWeapon(WeaponType weaponType)
        {
            var currentWeapon = _equipedWeapons.FirstOrDefault(cw => cw.WeaponType == _currentWeaponType);
            if (currentWeapon != null)
            {
                currentWeapon.gameObject.SetActive(false);
            }
            
            if (weaponType == WeaponType.NONE)
            {
                if (currentWeapon != null)
                {
                    currentWeapon.gameObject.SetActive(false);
                }

                _currentWeapon = null;
                _currentWeaponType = WeaponType.NONE;
                return;
            }
            
            var equippedWeapon = _equipedWeapons.FirstOrDefault(ew => ew.WeaponType == weaponType);
            if (equippedWeapon != null)
            {
                equippedWeapon.gameObject.SetActive(true);
                _currentWeapon = equippedWeapon;
            }
            else
            {
                var newWeapon = CreateWeaponByType(weaponType);
                _currentWeapon = newWeapon;
                _equipedWeapons.Add(newWeapon);

                if (newWeapon is IPunWeapon punWeapon)
                {
                    punWeapon.IsMine = photonView.IsMine;
                }
            }
            
            _currentWeaponType = weaponType;
        }

        private void ControlWeaponLoop()
        {
            if (!_isCanEquipWeapon)
                return;
            //Control weapon switching
            if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch)
                || Input.GetKeyDown(KeyCode.LeftAlt))
            {
                var maxWeaponIndex = Enum.GetNames(typeof(WeaponType)).Length - 1;
                var currentIndex = (int) _currentWeaponType;
                currentIndex++;
                if (currentIndex > maxWeaponIndex)
                {
                    currentIndex = 0;
                }
                Debug.Log($"Switch weapon for: {(WeaponType)currentIndex}" );
                photonView.RPC("SwitchWeaponRPC", RpcTarget.All, currentIndex);
            }
            
            //Control shot
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch)
                || Input.GetKeyDown(KeyCode.Space))
            {
                photonView.RPC("ShotWeaponRPC", RpcTarget.All);
            }
        }

        [PunRPC]
        public void ShotWeaponRPC()
        {
            if (_currentWeapon != null)
            {
                _currentWeapon.Shot();
            }
        }

        private void OnDeathLocal()
        {
            photonView.RPC("PlayerDeathRPC", RpcTarget.All);
        }

        [ContextMenu("Ressurection player DEBUG")]
        public void ResurrectionDEBUG()
        {
            ResurrectionLocal();
        }
        
        public void ResurrectionLocal()
        {
            photonView.RPC("ResurrectionRPC", RpcTarget.All);
        }

        [PunRPC]
        public void ResurrectionRPC()
        {
            _isCanEquipWeapon = true;
            _deathController.Resurrection();
            _playerUnitStats.ResurrectUnit();
        }
        
        [PunRPC]
        public void PlayerDeathRPC()
        {
            if (_playerHumanoidControl == null)
            {
                Debug.LogError($"{PlayerID}Player humanoid is missing!");
                return;
            }
            Debug.Log($"Player {PlayerID} DEATH!!!...");
            _deathController.Death();
            _isCanEquipWeapon = false;
            SwitchWeapon(WeaponType.NONE);
        }

        private void Update()
        {
            if (photonView.IsMine)
            {
                ControlWeaponLoop();
            }
        }

        #region Init
#if hPHOTON2
        public override void OnEnable() {
            base.OnEnable();
            PhotonNetwork.AddCallbackTarget(this);
        }

        public override void OnDisable() {
            base.OnDisable();
            PhotonNetwork.RemoveCallbackTarget(this);
        }
#endif
        public void Awake() {
            DontDestroyOnLoad(this);
            HumanoidNetworking.Start(debug, syncFingerSwing);
        }
        #endregion

        #region Start
        /// <summary>
        /// Init local player for another players when instantiate
        /// </summary>
        /// <param name="info"></param>
        public void OnPhotonInstantiate(PhotonMessageInfo info) {
#if hPHOTON2
            if (photonView.IsMine) {
#else
            if (photonView.isMine) {
#endif
                _currentColorIndex = RandomColorIndex;
                isLocal = true;
                name = "HumanoidPun(Local)";

                humanoids = HumanoidNetworking.FindLocalHumanoids();
                if (debug <= HumanoidNetworking.Debug.Info)
                    PhotonLog("Found " + humanoids.Count + " Humanoids");

                if (humanoids.Count <= 0)
                    return;

                foreach (HumanoidControl humanoid in humanoids) {
#if hPHOTON2
                    humanoid.nwId = photonView.ViewID;
#else
                    humanoid.nwId = photonView.viewID;
#endif
                    humanoid.humanoidNetworking = this;

                    if (debug <= HumanoidNetworking.Debug.Info)
                        Debug.Log(humanoid.nwId + ": Send Start Humanoid " + humanoid.humanoidId);
                    
                    InitializePlayerLocally(humanoid);
                    
                    
#if hPHOTON2
                    photonView.RPC("RpcStartHumanoid",
                        RpcTarget.Others,
                        humanoid.nwId,
                        humanoid.humanoidId,
                        humanoid.gameObject.name,
                        humanoid.remoteAvatar.name,
                        humanoid.transform.position,
                        humanoid.transform.rotation,
                        humanoid.physics,
                        _currentColorIndex);
                    photonView.RPC("InitWeaponRPC", RpcTarget.Others, CurrentWeaponIndex);
                    photonView.RPC("InitHumanoidRPC", RpcTarget.Others);
#else
                    photonView.RPC("RpcStartHumanoid", PhotonTargets.Others, humanoid.nwId, humanoid.humanoidId, humanoid.gameObject.name, humanoid.remoteAvatar.name, humanoid.transform.position, humanoid.transform.rotation, humanoid.physics);
#endif
                }

                NetworkingSpawner spawner = FindObjectOfType<NetworkingSpawner>();
                if (spawner != null)
                    spawner.OnNetworkingStarted();
            }
        }

        private void InitializePlayerLocally(HumanoidControl humanoidControl)
        {
            _playerHumanoidControl = humanoidControl;
            var meshColorChanger = humanoidControl.GetComponentInChildren<SkinnedMeshColorChanger>();
            if (meshColorChanger != null)
            {
                meshColorChanger.SetColor(_playerColors[_currentColorIndex]);
            }

            var fullBodyColorChanger = _playerHumanoidControl.GetComponentInChildren<FullBodyColorChanger>();
            Debug.Assert(fullBodyColorChanger != null, "fullBodyColorChanger != null Locally");
            if (fullBodyColorChanger != null)
            {
                fullBodyColorChanger.InitBasicColors();
            }
            _rightHandTransform = humanoidControl.transform.Find(_defaultRightHandTransformName);
            
            SwitchWeapon(_currentWeaponType);
            InitHumanoidGlobally();
            _playerUnitStats.OnDeath += OnDeathLocal;
        }

        /// <summary>
        /// Init for local player and remote player!
        /// </summary>
        private void InitHumanoidGlobally()
        {
            _playerUnitStats = _playerHumanoidControl.GetComponentInChildren<UnitStats>();
            if (_playerUnitStats == null)
            {
                Debug.LogError($"Cant recognize UnitStats on {_playerHumanoidControl.gameObject.name}!!");
            }

            _deathController = _playerHumanoidControl.GetComponent<DeathController>();
            Debug.Assert(_deathController != null, "_deathController != null");

            var controllers = new List<IController>
            {
                _deathController
            };
            
            InitControllers(controllers);

            OnPlayerInitialized(_playerHumanoidControl, _playerUnitStats);
            Debug.Log($"Player {PlayerID} initialized globally!");
            
        }

        private void InitControllers(List<IController> controllers)
        {
            foreach (var controller in controllers)
            {
                Debug.Assert(controller != null, "controller != null");
                controller?.InitializeController();
            }
        }
        

#if hPHOTON2
        public override void OnPlayerEnteredRoom(Player newPlayer) {
#else
        public void OnPhotonPlayerConnected(PhotonPlayer player) {
#endif
            List<HumanoidControl> humanoids = HumanoidNetworking.FindLocalHumanoids();
            if (humanoids.Count <= 0)
                return;

            foreach (HumanoidControl humanoid in humanoids) {
                if (debug <= HumanoidNetworking.Debug.Info)
                    Debug.Log(humanoid.nwId + ": (Re)Send StartHumanoid " + humanoid.humanoidId);

                // Notify new player about my humanoid
#if hPHOTON2
                photonView.RPC("RpcStartHumanoid",
                    RpcTarget.Others, 
                    humanoid.nwId, 
                    humanoid.humanoidId,
                    humanoid.gameObject.name, 
                    humanoid.remoteAvatar.name, 
                    humanoid.transform.position, 
                    humanoid.transform.rotation, 
                    humanoid.physics,
                    _currentColorIndex);
                
                photonView.RPC("InitWeaponRPC", RpcTarget.Others, CurrentWeaponIndex);
                photonView.RPC("InitHumanoidRPC", RpcTarget.Others);
#else
                photonView.RPC("RpcStartHumanoid", PhotonTargets.Others, humanoid.nwId, humanoid.humanoidId, humanoid.gameObject.name, humanoid.remoteAvatar.name, humanoid.transform.position, humanoid.transform.rotation, humanoid.physics);
#endif
                if (humanoid.leftHandTarget.grabbedObject != null)
                    humanoid.humanoidNetworking.Grab(humanoid.leftHandTarget, humanoid.leftHandTarget.grabbedObject, false);
                if (humanoid.rightHandTarget.grabbedObject != null)
                    humanoid.humanoidNetworking.Grab(humanoid.rightHandTarget, humanoid.rightHandTarget.grabbedObject, false);

            }
        }
        #endregion

        #region Update
        PhotonStream stream;

        private float lastPoseTime;
        private float lastReceiveTime;
        private Vector3 lastReceivedPosition;

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
            this.stream = stream;
#if hPHOTON2
            if (stream.IsWriting) {
#else
            if (stream.isWriting) {
#endif
                SendAvatarPose(stream);
            }
            else {
                ReceiveAvatarPose(stream);
            }
        }

        #endregion

        #region Stop
        private void OnDestroy() {
            if (debug <= HumanoidNetworking.Debug.Info)
                PhotonLog("Destroy Remote Humanoids");

            foreach (HumanoidControl humanoid in humanoids) {
                if (humanoid == null)
                    continue;

                if (humanoid.isRemote) {
                    if (humanoid.gameObject != null)
                        Destroy(humanoid.gameObject);
                }
                else
                    humanoid.nwId = 0;
            }
        }
        #endregion

        #region Instantiate Humanoid
        void IHumanoidNetworking.InstantiateHumanoid(HumanoidControl humanoid) {
            if (debug <= HumanoidNetworking.Debug.Info)
                PhotonLog("Send Instantiate Humanoid " + humanoid.humanoidId);

            humanoids.Add(humanoid);
#if hPHOTON2
            humanoid.nwId = photonView.ViewID;
#else
            humanoid.nwId = photonView.viewID;
#endif

#if hPHOTON2
            photonView.RPC("RpcStartHumanoid", RpcTarget.Others,
                humanoid.nwId, humanoid.humanoidId,
                humanoid.gameObject.name, humanoid.remoteAvatar.name,
                humanoid.transform.position, humanoid.transform.rotation,
                humanoid.physics,
                _currentColorIndex);
#else
            photonView.RPC("RpcStartHumanoid", PhotonTargets.Others,
                humanoid.nwId, humanoid.humanoidId,
                humanoid.gameObject.name, humanoid.remoteAvatar.name,
                humanoid.transform.position, humanoid.transform.rotation,
                humanoid.physics);
#endif
        }
        #endregion

        #region Destroy Humanoid
        void IHumanoidNetworking.DestroyHumanoid(HumanoidControl humanoid) {
            if (debug <= HumanoidNetworking.Debug.Info)
                Debug.Log(humanoid.nwId + ": Destroy Humanoid " + humanoid.humanoidId);

            humanoids.Remove(humanoid);
            humanoid.nwId = 0;

#if hPHOTON2
            if (PhotonNetwork.IsConnected)
                photonView.RPC("RpcDestroyHumanoid", RpcTarget.Others, humanoid.nwId, humanoid.humanoidId);
#else
            if (PhotonNetwork.connected)
                photonView.RPC("RpcDestroyHumanoid", PhotonTargets.Others, humanoid.nwId, humanoid.humanoidId);
#endif
        }

        [PunRPC]
        public void RpcDestroyHumanoid(int nwId, int humanoidId) {
            HumanoidControl remoteHumanoid = HumanoidNetworking.FindRemoteHumanoid(humanoids, humanoidId);
            if (remoteHumanoid == null) {
                // Unknown remote humanoid
                return;
            }

            if (remoteHumanoid.gameObject != null)
                Destroy(remoteHumanoid.gameObject);
        }
        #endregion

        #region Start Humanoid

        [PunRPC]
        public void InitHumanoidRPC()
        {
            var lastHumanoid = humanoids.LastOrDefault();
            _playerHumanoidControl = lastHumanoid;
            InitHumanoidGlobally();
        }

        [PunRPC]
        public void InitWeaponRPC(int weaponIndex)
        {
            var lastHumanoid = humanoids.LastOrDefault();
            if (lastHumanoid != null)
            {
                var hand = lastHumanoid.transform.Find(_defaultRightHandTransformName);
                if (hand != null)
                {
                    _rightHandTransform = hand;
                    SwitchWeaponRPC(weaponIndex);
                }
            }
        }
        
        [PunRPC]
        public void RpcStartHumanoid(
            int nwId, int humanoidId,
            string name, string avatarPrefabName,
            Vector3 position, Quaternion rotation,
            bool physics, int colorId) {
#if hPHOTON2
            if (nwId != photonView.ViewID)
#else
            if (nwId != photonView.viewID)
#endif
                return;

            HumanoidControl remoteHumanoid = HumanoidNetworking.FindRemoteHumanoid(humanoids, humanoidId);
            if (remoteHumanoid != null) {
                // This remote humanoid already exists
                return;
            }

            var humanoid = HumanoidNetworking.StartHumanoid(nwId, humanoidId, name, avatarPrefabName, position,
                rotation, physics);
            humanoids.Add(humanoid);

            var meshColorChanger = humanoid.GetComponentInChildren<SkinnedMeshColorChanger>();
            if (meshColorChanger != null)
            {
                meshColorChanger.SetColor(_playerColors[colorId]);
            }

            var fullBodyColorChanger = humanoid.GetComponentInChildren<FullBodyColorChanger>();
            Debug.Assert(fullBodyColorChanger != null, "fullBodyColorChanger != null from RPC");
            if (fullBodyColorChanger != null)
            {
                fullBodyColorChanger.InitBasicColors();
            }
            
        }
        #endregion

        #region Pose

        #region Send
        private void SendAvatarPose(PhotonStream stream) {
            this.stream = stream;

            Send(humanoids.Count);
            foreach (HumanoidControl humanoid in humanoids) {
                if (humanoid != null)
                    this.SendAvatarPose(humanoid);
            }
        }
        #endregion

        #region Receive

        PhotonStream reader;

        private void ReceiveAvatarPose(PhotonStream reader) {
            this.reader = reader;

        int humanoidsCount = ReceiveInt();
            for (int i = 0; i < humanoidsCount; i++) {
                int nwId = ReceiveInt();
#if hPHOTON2
                if (nwId != photonView.ViewID)
                    return;
#else
                if (nwId != photonView.viewID)
                    return;
#endif
                int humanoidId = ReceiveInt();

                HumanoidControl humanoid = HumanoidNetworking.FindRemoteHumanoid(humanoids, humanoidId);
                if (humanoid == null) {
                    if (debug <= HumanoidNetworking.Debug.Warning)
                        Debug.LogWarning(nwId + ": Could not find humanoid: " + humanoidId);
                    return;
                }

                this.ReceiveAvatarPose(humanoid, ref lastPoseTime, ref lastReceiveTime, ref lastReceivedPosition);
            }
        }

        #endregion

        #endregion

        #region Grab
        void IHumanoidNetworking.Grab(HandTarget handTarget, GameObject obj, bool rangeCheck) {
            if (handTarget.humanoid.isRemote)
                return;

            if (debug <= HumanoidNetworking.Debug.Info)
                Debug.Log(handTarget.humanoid.nwId + ": Grab " + obj);

            PhotonView objView = obj.GetComponent<PhotonView>();
            if (objView == null) {
                if (debug <= HumanoidNetworking.Debug.Error)
                    Debug.LogError("Photon Grab: Grabbed object does not have a PhotonView - " + obj.name);
            }
            else
#if hPHOTON2
                photonView.RPC("RpcGrab", RpcTarget.Others, handTarget.humanoid.humanoidId, objView.ViewID, handTarget.isLeft, rangeCheck);
#else
                photonView.RPC("RpcGrab", PhotonTargets.Others, handTarget.humanoid.humanoidId, objView.viewID, handTarget.isLeft, rangeCheck);
#endif
        }

        [PunRPC]
        public void RpcGrab(int humanoidId, int objViewID, bool isLeft, bool rangeCheck) {
            PhotonView objView = PhotonView.Find(objViewID);
            GameObject obj = objView.gameObject;

            if (debug <= HumanoidNetworking.Debug.Info)
                PhotonLog("RpcGrab " + obj);

            if (obj == null)
                return;

            HumanoidControl humanoid = HumanoidNetworking.FindRemoteHumanoid(humanoids, humanoidId);
            if (humanoid == null) {
                if (debug <= HumanoidNetworking.Debug.Warning)
                    PhotonLogWarning("Could not find humanoid: " + humanoidId);
                return;
            }

            HandTarget handTarget = isLeft ? humanoid.leftHandTarget : humanoid.rightHandTarget;
            if (handTarget != null) {
                HandInteraction.LocalGrab(handTarget, obj, rangeCheck);
            }
        }
        #endregion

        #region Let Go
        void IHumanoidNetworking.LetGo(HandTarget handTarget) {
            if (handTarget.humanoid.isRemote)
                return;

            if (debug <= HumanoidNetworking.Debug.Info)
                Debug.Log(handTarget.humanoid.nwId + ": LetGo ");

#if hPHOTON2
            photonView.RPC("RpcLetGo", RpcTarget.Others, handTarget.humanoid.humanoidId, handTarget.isLeft);
#else
            photonView.RPC("RpcLetGo", PhotonTargets.Others, handTarget.humanoid.humanoidId, handTarget.isLeft);
#endif
        }

        [PunRPC]
        public void RpcLetGo(int humanoidId, bool isLeft) {
            if (debug <= HumanoidNetworking.Debug.Info)
                PhotonLog("RpcLetGo");

            HumanoidControl humanoid = HumanoidNetworking.FindRemoteHumanoid(humanoids, humanoidId);
            if (humanoid == null) {
                if (debug <= HumanoidNetworking.Debug.Warning)
                    PhotonLogWarning("Could not find humanoid: " + humanoidId);
                return;
            }

            HandTarget handTarget = isLeft ? humanoid.leftHandTarget : humanoid.rightHandTarget;
            if (handTarget != null) {
                HandInteraction.LocalLetGo(handTarget);
            }
        }
        #endregion

        #region Change Avatar
        void IHumanoidNetworking.ChangeAvatar(HumanoidControl humanoid, string avatarPrefabName) {
            if (debug <= HumanoidNetworking.Debug.Info)
                Debug.Log(humanoid.nwId + ": Change Avatar: " + avatarPrefabName);

#if hPHOTON2
            photonView.RPC("RpcChangeAvatar", RpcTarget.Others, humanoid.humanoidId, avatarPrefabName);
#else
            photonView.RPC("RpcChangeAvatar", PhotonTargets.Others, humanoid.humanoidId, avatarPrefabName);
#endif
        }

        [PunRPC]
        public void RpcChangeAvatar(int humanoidId, string avatarPrefabName) {
            if (debug <= HumanoidNetworking.Debug.Info)
                PhotonLog("RpcChangeAvatar " + avatarPrefabName);

            HumanoidControl humanoid = HumanoidNetworking.FindRemoteHumanoid(humanoids, humanoidId);
            if (humanoid == null) {
                if (debug <= HumanoidNetworking.Debug.Warning)
                    PhotonLogWarning("Could not find humanoid: " + humanoidId);
                return;
            }

            GameObject remoteAvatar = (GameObject)Resources.Load(avatarPrefabName);
            if (remoteAvatar == null) {
                if (debug <= HumanoidNetworking.Debug.Error)
                    PhotonLogError("Could not load remote avatar " + avatarPrefabName + ". Is it located in a Resources folder?");
                return;
            }
            humanoid.LocalChangeAvatar(remoteAvatar);
        }
        #endregion

        #region Network Sync

        void IHumanoidNetworking.ReenableNetworkSync(GameObject obj) {
            PhotonTransformView transformView = obj.GetComponent<PhotonTransformView>();
            if (transformView != null) {
#if hPHOTON2
                transformView.m_SynchronizePosition = true;
                transformView.m_SynchronizeRotation = true;
#else
                transformView.m_PositionModel.SynchronizeEnabled = true;
                transformView.m_RotationModel.SynchronizeEnabled = true;
#endif
            }
        }

        void IHumanoidNetworking.DisableNetworkSync(GameObject obj) {
            PhotonTransformView transformView = obj.GetComponent<PhotonTransformView>();
            if (transformView != null) {
#if hPHOTON2
                transformView.m_SynchronizePosition = false;
                transformView.m_SynchronizeRotation = false;
#else
                transformView.m_PositionModel.SynchronizeEnabled = false;
                transformView.m_RotationModel.SynchronizeEnabled = false;
#endif
            }
        }

#endregion

                #region Send
        public void Send(bool b) { stream.SendNext(b); }
        public void Send(byte b) { stream.SendNext(b); }
        public void Send(int x) { stream.SendNext(x); }
        public void Send(float f) { stream.SendNext(f); }
        public void Send(Vector3 v) { stream.SendNext(v); }
        public void Send(Quaternion q) { stream.SendNext(q); }
                #endregion

                #region Receive
        public bool ReceiveBool() { return (bool)reader.ReceiveNext(); }
        public byte ReceiveByte() { return (byte)reader.ReceiveNext(); }
        public int ReceiveInt() { return (int)reader.ReceiveNext(); }
        public float ReceiveFloat() { return (float)reader.ReceiveNext(); }
        public Vector3 ReceiveVector3() { return (Vector3)reader.ReceiveNext(); }
        public Quaternion ReceiveQuaternion() { return (Quaternion)reader.ReceiveNext(); }
                #endregion

        private void PhotonLog(string message) {
#if hPHOTON2
            Debug.Log(photonView.ViewID + ": " + message);
#else
            Debug.Log(photonView.viewID + ": " + message);
#endif
        }

        private void PhotonLogWarning(string message) {
#if hPHOTON2
            Debug.LogWarning(photonView.ViewID + ": " + message);
#else
            Debug.LogWarning(photonView.viewID + ": " + message);
#endif
        }

        private void PhotonLogError(string message) {
#if hPHOTON2
            Debug.LogError(photonView.ViewID + ": " + message);
#else
            Debug.LogError(photonView.viewID + ": " + message);
#endif
        }

#endif
            }
        }
