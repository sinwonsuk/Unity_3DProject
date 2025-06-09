using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine.SceneManagement;

public class AutoMatchManager : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private int inGameSceneIndex = 2;
    private NetworkRunner runner;
    private int maxPlayers = 2;
    private bool isMatching = false;

    private async void Start()
    {
        runner = RunnerSingleton.Instance;

        if (runner == null)
        {
            Debug.LogError(" NetworkRunner �ν��Ͻ��� ����x");
            return;
        }

        if (runner.IsRunning)
        {
            Debug.Log(" runner�� �̹� ���� ��. Shutdown �õ�");
            await runner.Shutdown();

            // Shutdown �� �����
            Debug.Log(" runner ����� ��");
            RunnerSingleton.ClearRunner(); 
            runner = RunnerSingleton.CreateRunner(); // �� runner ����
        }

        runner.ProvideInput = true;
        runner.AddCallbacks(this);
    }
    public void StartAutoMatching()
    {
        if (isMatching) return;

        Debug.Log(" ��Ī ���� ");
        _ = TryJoinOrCreateRoom();  
    }

    private async Task TryJoinOrCreateRoom()
    {
        if (isMatching) return;
        isMatching = true;

        runner = RunnerSingleton.Instance;

        var result = await runner.JoinSessionLobby(SessionLobby.ClientServer);
        if (!result.Ok)
        {
            Debug.LogError($"�κ� ���� ����: {result.ShutdownReason}");
            return;
        }

        Debug.Log("�κ� ���� ����");
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        foreach (var session in sessionList)
        {
            if (session.PlayerCount < maxPlayers)
            {
                Debug.Log($"�� �� �߰�: {session.Name}, ���� �õ�");
                StartGame(session.Name);
                return;
            }
        }

        string newRoom = "Room_" + Random.Range(1000, 9999);
        Debug.Log($"�� �� ����, �� �� ����: {newRoom}");
        StartGame(newRoom);
    }

    private async void StartGame(string sessionName)
    {
        var result = await runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = sessionName,
            Scene = SceneRef.FromIndex(2),
            SceneManager = runner.GetComponent<NetworkSceneManagerDefault>()
        });

        if (result.Ok)
        {
            Debug.Log($"'{sessionName}' ��Ī ����");
        }
        else
        {
            Debug.LogError($"'{sessionName}' ��Ī ����: {result.ShutdownReason}");
        }
    }

    public async void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        var currentSessionName = runner.SessionInfo?.Name ?? "�̸� ����";
        int currentPlayerCount = runner.ActivePlayers.Count();

        Debug.Log($"�÷��̾� {player.PlayerId} ��(��) �� '{currentSessionName}'�� ����.");
        Debug.Log($"���� '{currentSessionName}' �� �ο� ��: {currentPlayerCount}");

        if (currentPlayerCount >= maxPlayers && runner.IsServer)
        {
            Debug.Log("�÷��̾� 2�� Ȯ�ε�. Host�� �ΰ��� �� �ε� �õ�!");
            Debug.Log("�� �ε� �õ�");

            await runner.SceneManager.LoadScene(
                SceneRef.FromIndex(2),
                new NetworkLoadSceneParameters() // �⺻��: LoadSceneMode.Single
            );

            Debug.Log("�� �ε� �Ϸ��?");
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, System.IO.Stream stream) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, System.ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
}
