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
//        Debug.Log("[HostMigrationHandler] HostMigrationToken ���� ? �� Runner ����");

//        var runnerPrefab = RunnerSingleton.GetRunnerPrefab();
//        if (runnerPrefab == null)
//        {
//            Debug.LogError("RunnerPrefab�� ã�� ���߽��ϴ� ? RunnerSingleton�� ������ ���� �ʿ�");
//            return;
//        }

//        var newGO = Instantiate(runnerPrefab);
//        var newRunner = newGO.GetComponent<NetworkRunner>();
//        var sceneMgr = newGO.GetComponent<NetworkSceneManagerDefault>();
//        if (sceneMgr == null)
//            sceneMgr = newGO.AddComponent<NetworkSceneManagerDefault>();

//        newRunner.ProvideInput = true;
//        newRunner.AddCallbacks(this);  // �߿�

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
//            Debug.LogError($"HostMigration StartGame ����: {result.ShutdownReason}");
//            return;
//        }

//        RunnerSingleton.ReplaceInstance(newRunner);
//    }

//    public void OnSceneLoadDone(NetworkRunner runner)
//    {
//        Debug.Log("[HostMigrationHandler] OnSceneLoadDone ȣ���");

//        if (!runner.IsServer)
//        {
//            Debug.Log("[HostMigrationHandler] Ŭ���̾�Ʈ�� ������ �Ϸ� (�� ȣ��Ʈ �ƴ�)");
//            return;
//        }

//        Debug.Log("[HostMigrationHandler] �� ȣ��Ʈ �°� ? ������ ������Ʈ ���� ����");

//        var spawner = BasicSpawner2.Instance;
//        if (spawner == null)
//        {
//            Debug.LogWarning("[HostMigrationHandler] BasicSpawner2 �ν��Ͻ��� ã�� ���߽��ϴ�");
//            return;
//        }

//        foreach (var obj in runner.GetResumeSnapshotNetworkObjects())
//        {
//            var owner = obj.InputAuthority;
//            if (owner == PlayerRef.None) continue;

//            spawner.RegisterRestoredObject(owner, obj);

//            Debug.Log($"������ ��ü: {obj.name} | InputAuth={owner.PlayerId} | StateAuth={(obj.HasStateAuthority ? "��" : "�ƴ�")}");
//        }
//    }

//    // �Ʒ� �ݹ��� ����� ����
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