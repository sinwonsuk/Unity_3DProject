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
            Debug.LogError(" NetworkRunner 인스턴스가 존재x");
            return;
        }

        if (runner.IsRunning)
        {
            Debug.Log(" runner가 이미 실행 중. Shutdown 시도");
            await runner.Shutdown();

            // Shutdown 후 재생성
            Debug.Log(" runner 재생성 중");
            RunnerSingleton.ClearRunner(); 
            runner = RunnerSingleton.CreateRunner(); // 새 runner 생성
        }

        runner.ProvideInput = true;
        runner.AddCallbacks(this);
    }
    public void StartAutoMatching()
    {
        if (isMatching) return;

        Debug.Log(" 매칭 시작 ");
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
            Debug.LogError($"로비 진입 실패: {result.ShutdownReason}");
            return;
        }

        Debug.Log("로비 진입 성공");
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        foreach (var session in sessionList)
        {
            if (session.PlayerCount < maxPlayers)
            {
                Debug.Log($"빈 방 발견: {session.Name}, 접속 시도");
                StartGame(session.Name);
                return;
            }
        }

        string newRoom = "Room_" + Random.Range(1000, 9999);
        Debug.Log($"빈 방 없음, 새 방 생성: {newRoom}");
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
            Debug.Log($"'{sessionName}' 매칭 성공");
        }
        else
        {
            Debug.LogError($"'{sessionName}' 매칭 실패: {result.ShutdownReason}");
        }
    }

    public async void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        var currentSessionName = runner.SessionInfo?.Name ?? "이름 없음";
        int currentPlayerCount = runner.ActivePlayers.Count();

        Debug.Log($"플레이어 {player.PlayerId} 이(가) 방 '{currentSessionName}'에 입장.");
        Debug.Log($"현재 '{currentSessionName}' 방 인원 수: {currentPlayerCount}");

        if (currentPlayerCount >= maxPlayers && runner.IsServer)
        {
            Debug.Log("플레이어 2명 확인됨. Host가 인게임 씬 로드 시도!");
            Debug.Log("씬 로드 시도");

            await runner.SceneManager.LoadScene(
                SceneRef.FromIndex(2),
                new NetworkLoadSceneParameters() // 기본값: LoadSceneMode.Single
            );

            Debug.Log("씬 로드 완료됨?");
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
