using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System;
using ExitGames.Client.Photon.StructWrapping;
using Cinemachine;
using static Unity.Collections.Unicode;
using Unity.VisualScripting.Antlr3.Runtime;
using System.Collections;
using Unity.VisualScripting;

/// Host(서버)가 모든 캐릭터를 Spawn 하고, 각 플레이어는 InputAuthority 를 받아서 자신의 캐릭터만 조종
[RequireComponent(typeof(NetworkObject))]
public class BasicSpawner2 : NetworkBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private CharacterPrefabData prefabData;   // 캐릭터 이름 = 프리팹 찾기 
    [SerializeField] private SpawnManager spawnManager;  // 스폰 위치

    [SerializeField]
    private CinemachineVirtualCamera cam;
    [SerializeField]
    private float lookSpeed = 10.0f;
    int a = 0;
    private CinemachinePOV _pov;
    private float _prevYaw;
    private float _prevPitch;

    private bool _mouseButton0;
    private bool _mouseButton1;

    // 중복 스폰 방지
    private readonly Dictionary<PlayerRef, NetworkObject> _spawned = new();
    private readonly HashSet<PlayerRef> _alreadySpawned = new();
    public static BasicSpawner2 Instance;

    private bool _spawnDone = false;

    private void OnEnable()
    {
        Instance = this;
    }

    private void OnDisable()
    {
        if (Instance == this)
            Instance = null;
    }

    public void DespawnSelf()
    {
        var myRef = Runner.LocalPlayer;

        if (_spawned.TryGetValue(myRef, out var obj))
        {
            Debug.Log($"[DespawnSelf] 내 캐릭터 오브젝트 Despawn 처리");
            Runner.Despawn(obj);
            _spawned.Remove(myRef);
        }
    }

    private void Awake()
    {
    //    _needResume = false;
        _pov = cam.GetCinemachineComponent<CinemachinePOV>();
        // 초기값 캡처
        _prevYaw = _pov.m_HorizontalAxis.Value;
        _prevPitch = _pov.m_VerticalAxis.Value;
    }

    private void Update()
    {
        _mouseButton0 = _mouseButton0 | Input.GetMouseButton(0);
        _mouseButton1 = _mouseButton1 | Input.GetMouseButton(1);
    }

    public override void Spawned()
    {

        Runner.AddCallbacks(this);

        // 1회만 실행되도록 전역 플래그 검사
        if (_spawnDone)
        {
            Debug.Log("[Spawned] 이미 TrySpawn 완료됨 – 스킵");
            return;
        }

        // HostMigration 복원된 경우도 TrySpawn 금지
        if (_spawned.ContainsKey(Runner.LocalPlayer))
        {
            Debug.Log("[Spawned] 이미 복원된 오브젝트 존재 – TrySpawn 생략");
            _spawnDone = true;  // 복원된 것도 "스폰 완료"로 간주
            return;
        }

        if (_alreadySpawned.Contains(Runner.LocalPlayer))
        {
            Debug.Log("[Spawned] TrySpawn 이력 있음 – 스킵");
            _spawnDone = true;
            return;
        }

        if (Runner.IsServer)
        {
            TrySpawn(Runner.LocalPlayer, GetMyCharacterName());
        }
        else
        {
            string myName = GetMyCharacterName();
            if (!string.IsNullOrEmpty(myName))
                RPC_RequestSpawn(Runner.LocalPlayer, myName);
        }

        _spawnDone = true; // 
    }

    /// 모든 클라이언트(RpcSources.All)가 호출 가능, 서버(StateAuthority)가 처리
    [Rpc(sources: RpcSources.All, targets: RpcTargets.StateAuthority)]
    private void RPC_RequestSpawn(PlayerRef sender, string characterName)
    {
        if (!Runner.IsServer) return; 
        TrySpawn(sender, characterName);
    }

    private void TrySpawn(PlayerRef target, string characterName)
    {
        //if (_spawned.ContainsKey(target)) return; // 이미 생성 됐으면 건너뜀

        //var prefabRef = FindPrefab(characterName);
        //if (prefabRef == NetworkPrefabRef.Empty)
        //{
        //    Debug.LogError($"{characterName} 프리팹을 찾지 못함");
        //    return;
        //}

        //Vector3 pos = spawnManager.GetNextSpawnPosition();
        //var obj = Runner.Spawn(prefabRef, pos, Quaternion.identity, target);
        //_spawned[target] = obj;
        //Debug.Log($"스폰 완료 ▶ {characterName} (Player {target.PlayerId})");
        if (_alreadySpawned.Contains(target))
        {
            Debug.LogWarning($"[TrySpawn] Player {target.PlayerId}는 이미 스폰된 적 있음 – 스폰 생략");
            return;
        }

        if (_spawned.ContainsKey(target))
        {
            Debug.LogWarning($"[TrySpawn] Player {target.PlayerId}는 이미 스폰되어 있음 – 중복 방지");
            return;
        }

        var prefabRef = FindPrefab(characterName);
        if (prefabRef == NetworkPrefabRef.Empty)
        {
            Debug.LogError($"{characterName} 프리팹을 찾지 못함");
            return;
        }

        Vector3 pos = spawnManager.GetNextSpawnPosition();
        var obj = Runner.Spawn(prefabRef, pos, Quaternion.identity, target);
        _spawned[target] = obj;
        _alreadySpawned.Add(target); // 스폰 이력 기록
    }

    private NetworkPrefabRef FindPrefab(string name)
    {
        foreach (var e in prefabData.characterPrefabs)
            if (e.characterName == name) 
                return e.prefab;

        if (prefabData.characterPrefabs.Count > 0)
        {
            return prefabData.characterPrefabs[0].prefab;
        }
        else
        {
            return NetworkPrefabRef.Empty;
        }
    }

    private string GetMyCharacterName()
    {
        string n = MatchQueueManager.Instance?.MySelectedCharacterName;
        return string.IsNullOrEmpty(n) ? "Soldier_James" : n;
    }

    public void OnPlayerJoined(NetworkRunner r, PlayerRef p) { }
    public void OnPlayerLeft(NetworkRunner r, PlayerRef p)
    {
        if (!r.IsServer) return;
        if (_spawned.TryGetValue(p, out var obj))
        {
            r.Despawn(obj);
            _spawned.Remove(p);
        }
    }
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {

        var data = new NetworkInputData();

        data.moveAxis = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (Input.GetKey(KeyCode.W))
            data.direction += Vector3.forward;

        if (Input.GetKey(KeyCode.S))
            data.direction += Vector3.back;

        if (Input.GetKey(KeyCode.A))
            data.direction += Vector3.left;

        if (Input.GetKey(KeyCode.D))
            data.direction += Vector3.right;

        data.CameraRotateY = Camera.main.transform.eulerAngles.y;

        // 2) POV 절대값 → 델타로 변환
        float curYaw = _pov.m_HorizontalAxis.Value;
        float curPitch = _pov.m_VerticalAxis.Value;

        data.LookRotationDelta = new Vector2(
            curYaw,
             -curPitch
        );




        data.CameraForward = Camera.main.transform.forward;

        data.buttons.Set(NetworkInputData.KEY_C, Input.GetKey(KeyCode.C));
        data.buttons.Set(NetworkInputData.KEY_SPACE, Input.GetKey(KeyCode.Space));
        data.buttons.Set(NetworkInputData.MOUSEBUTTON1, Input.GetMouseButton(1));
        data.buttons.Set(NetworkInputData.MOUSEBUTTON0, Input.GetMouseButton(0));
        data.buttons.Set(NetworkInputData.KEY_L, Input.GetKey(KeyCode.L));
        data.buttons.Set(NetworkInputData.KEY_CTRL, Input.GetKey(KeyCode.LeftControl));

        data.buttons.Set(NetworkInputData.NUM_1, Input.GetKey(KeyCode.Alpha1));
        data.buttons.Set(NetworkInputData.NUM_2, Input.GetKey(KeyCode.Alpha2));
        data.buttons.Set(NetworkInputData.NUM_3, Input.GetKey(KeyCode.Alpha3));
        data.buttons.Set(NetworkInputData.NUM_4, Input.GetKey(KeyCode.Alpha4));
        data.buttons.Set(NetworkInputData.NUM_5, Input.GetKey(KeyCode.Alpha5));
        data.buttons.Set(NetworkInputData.NUM_6, Input.GetKey(KeyCode.Alpha6));


        _mouseButton0 = false;
        _mouseButton1 = false;
        input.Set(data);
    }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) {

        Debug.Log($"OnShutdown 호출 – Reason = {shutdownReason}");
        if (shutdownReason == ShutdownReason.HostMigration)
            Debug.Log(" → HostMigration 으로 종료됨");
        else
            Debug.Log(" → 일반 종료됨");
    }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public async void OnHostMigration(NetworkRunner runner, HostMigrationToken token)
    {

    }


    public void OnSceneLoadDone(NetworkRunner runner) {

    }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

}
