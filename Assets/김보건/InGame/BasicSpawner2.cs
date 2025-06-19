using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System;
using ExitGames.Client.Photon.StructWrapping;
using Cinemachine;
using static Unity.Collections.Unicode;

/// Host(서버)가 모든 캐릭터를 Spawn 하고, 각 플레이어는 InputAuthority 를 받아서 자신의 캐릭터만 조종
[RequireComponent(typeof(NetworkObject))]
public class BasicSpawner2 : NetworkBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private CharacterPrefabData prefabData;   // 캐릭터 이름 = 프리팹 찾기 
    [SerializeField] private SpawnManager spawnManager;  // 스폰 위치

    // 중복 스폰 방지
    private readonly Dictionary<PlayerRef, NetworkObject> _spawned = new();

    public override void Spawned()
    {
        // 호스트 : 자기 자신 캐릭터 먼저 스폰
        if (Runner.IsServer)
        {
            TrySpawn(Runner.LocalPlayer, GetMyCharacterName());
        }
        // 클라이언트 : 선택한 캐릭터를 서버에게 요청
        else
        {
            string myName = GetMyCharacterName();
            Debug.Log($"[Client] 선택 캐릭터 = {myName}");

            if (!string.IsNullOrEmpty(myName))
                RPC_RequestSpawn(Runner.LocalPlayer, myName); // PlayerRef 전달
            else
                Debug.LogWarning("[Client] 캐릭터 이름이 비어 있어 Spawn 요청을 건너뜀");
        }
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
        if (_spawned.ContainsKey(target)) return; // 이미 생성 됐으면 건너뜀

        var prefabRef = FindPrefab(characterName);
        if (prefabRef == NetworkPrefabRef.Empty)
        {
            Debug.LogError($"{characterName} 프리팹을 찾지 못함");
            return;
        }

        Vector3 pos = spawnManager.GetNextSpawnPosition();
        var obj = Runner.Spawn(prefabRef, pos, Quaternion.identity, target);
        _spawned[target] = obj;
        Debug.Log($"스폰 완료 ▶ {characterName} (Player {target.PlayerId})");
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
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
}