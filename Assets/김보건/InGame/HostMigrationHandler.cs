//// HostMigrationHandler.cs
//using Fusion;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using System.Collections.Generic;
//using System;

//public class HostMigrationHandler : MonoBehaviour, INetworkRunnerCallbacks
//{
//    private NetworkRunner _runner;

//    private void Start()
//    {
//        if (_runner != null)
//            _runner.AddCallbacks(this);
//    }

//    public async void OnHostMigration(NetworkRunner runner, HostMigrationToken token)
//    {
//        Debug.Log("[HostMigrationHandler] HostMigrationToken 수신 ? 새 Runner 생성");

//        var runnerPrefab = RunnerSingleton.GetRunnerPrefab();
//        if (runnerPrefab == null)
//        {
//            Debug.LogError("RunnerPrefab을 찾지 못했습니다 ? RunnerSingleton에 프리팹 연결 필요");
//            return;
//        }

//        var newGO = Instantiate(runnerPrefab);
//        var newRunner = newGO.GetComponent<NetworkRunner>();
//        var sceneMgr = newGO.GetComponent<NetworkSceneManagerDefault>();
//        if (sceneMgr == null)
//            sceneMgr = newGO.AddComponent<NetworkSceneManagerDefault>();

//        newRunner.ProvideInput = true;
//        newRunner.AddCallbacks(this);  // 중요

//        var curScene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);

//        var result = await newRunner.StartGame(new StartGameArgs()
//        {
//            HostMigrationToken = token,
//            GameMode = GameMode.AutoHostOrClient,
//            Scene = curScene,
//            SceneManager = sceneMgr
//        });

//        if (!result.Ok)
//        {
//            Debug.LogError($"HostMigration StartGame 실패: {result.ShutdownReason}");
//            return;
//        }

//        RunnerSingleton.ReplaceInstance(newRunner);
//    }

//    public void OnSceneLoadDone(NetworkRunner runner)
//    {
//        Debug.Log("[HostMigrationHandler] OnSceneLoadDone 호출됨");

//        if (!runner.IsServer)
//        {
//            Debug.Log("[HostMigrationHandler] 클라이언트로 재접속 완료 (새 호스트 아님)");
//            return;
//        }

//        Debug.Log("[HostMigrationHandler] 새 호스트 승격 ? 스냅샷 오브젝트 복원 시작");

//        var spawner = BasicSpawner2.Instance;
//        if (spawner == null)
//        {
//            Debug.LogWarning("[HostMigrationHandler] BasicSpawner2 인스턴스를 찾지 못했습니다");
//            return;
//        }

//        foreach (var obj in runner.GetResumeSnapshotNetworkObjects())
//        {
//            var owner = obj.InputAuthority;
//            if (owner == PlayerRef.None) continue;

//            spawner.RegisterRestoredObject(owner, obj);

//            Debug.Log($"복원된 객체: {obj.name} | InputAuth={owner.PlayerId} | StateAuth={(obj.HasStateAuthority ? "나" : "아님")}");
//        }
//    }

//    // 아래 콜백은 비워도 무방
//    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
//    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
//    public void OnInput(NetworkRunner runner, NetworkInput input) { }
//    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
//    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
//    public void OnConnectedToServer(NetworkRunner runner) { }
//    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
//    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
//    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
//    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
//    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
//    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
//    public void OnSceneLoadStart(NetworkRunner runner) { }
//    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
//    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
//    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
//    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
//}