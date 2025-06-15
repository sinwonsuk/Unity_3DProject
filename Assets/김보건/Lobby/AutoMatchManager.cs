using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Collections;
using static Unity.Collections.Unicode;

public class AutoMatchManager : MonoBehaviour, INetworkRunnerCallbacks
{

    private void Awake()
    {
        Instance = this;
    }

    public void OnMatchButtonClick()
    {
        matchTimerUI?.StartTimer();

        if (!MatchQueueManager.Instance.RoomStarted)
        {
            // 매칭 1등, 방 생성
            string roomName = $"Room_7777"; 
            MatchQueueManager.Instance.CurrentRoomName = roomName;

            Debug.Log($"방 생성 및 접속: {roomName}");
            StartGameWithRoomName(roomName);
        }
        else
        {
            // 매칭2등 동일한 방 입장
            string roomName = MatchQueueManager.Instance.CurrentRoomName;

            Debug.Log($"참가자 기존 방 입장: {roomName}");
            StartGameWithRoomName(roomName);
        }
    }

    public async void StartGameWithRoomName(string roomName)
    {
        runner = RunnerSingleton.Instance;
        runner.ProvideInput = true;
        runner.AddCallbacks(this);

        var sceneManager = runner.GetComponent<NetworkSceneManagerDefault>();

        if (sceneManager == null)
        {
            Debug.LogError("러너프리팹에 NetworkSceneManagerDefault가 없음");
        }
        else
        {
            Debug.Log($"씬 매니저 타입: {sceneManager.GetType().Name}");
        }

        // 현재 로비 씬 인덱스
        int lobbySceneIndex = UnityEngine.SceneManagement.SceneManager
                              .GetActiveScene().buildIndex;

        // 네트워크 씬 매니저에 로비 씬 등록
        SceneRef startScene = SceneRef.FromIndex(lobbySceneIndex);

        var result = await runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = roomName,
            Scene = startScene,
            SceneManager = sceneManager
        });

        if (result.Ok)
        {
            Debug.Log("클라이언트 StartGame 성공 ");
        }
        else
        {
            Debug.LogError($"클라이언트 StartGame 실패 : {result.ShutdownReason}");
        }
    }

    public async void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"플레이어 입장: {player}. 현재 인원: {runner.ActivePlayers.Count()}");

        if (runner.IsServer && runner.ActivePlayers.Count() == 2)
        {
            Debug.Log("인게임 씬으로 이동 시작");

            // 매칭 완료되면 현재 방 이름 초기화
            MatchQueueManager.Instance.CurrentRoomName = "";

            await runner.LoadScene(SceneRef.FromIndex(2), LoadSceneMode.Single);
        }
    }
    public void CancelMatch()
    {
        if (runner != null && runner.IsRunning)
        {
            Debug.Log("매칭 취소 중");
            runner.Shutdown(); // 네트워크 연결 종료
        }

        matchTimerUI?.StopTimer(); // 타이머 중지
        MatchQueueManager.Instance.CurrentRoomName = ""; // 방 초기화

        // 기존 러너 파괴 및 새로 생성
        RunnerSingleton.ClearRunner();
        runner = RunnerSingleton.CreateRunner();
        if (runner != null)
        {
            runner.ProvideInput = true;
            runner.AddCallbacks(this);
            Debug.Log("새 runner 재등록 완료");
        }

    }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
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
    public void OnSceneLoadDone(NetworkRunner runner)
    {
        Debug.Log("씬 로딩 완료");

        var boTest = SceneManager.GetSceneByName("BoTest");
        if (boTest.IsValid() && boTest.isLoaded)
        {
            // BoTest를 ActiveScene으로
            SceneManager.SetActiveScene(boTest);

            // 호스트라면 로비 씬 끄기
            if (runner.IsServer)
            {
                var lobby = SceneManager.GetSceneByName("LobbyScene");
                if (lobby.isLoaded)
                {
                    _ = runner.SceneManager.UnloadScene(SceneRef.FromIndex(1)); // 로비 인덱스
                }
            }

            // 로비 UI 끄기
            //LobbyUIManager.Instance?.gameObject.SetActive(false);

            Debug.Log("OnSceneLoadDone 인게임 씬 처리 완료");
        }
        else
        {
            // 첫 네트워크 연결 때 로비 씬 재로드
            Debug.Log("OnSceneLoadDone 로비 씬 로드 완료 /패스");
        }
    }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, System.ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

    private NetworkRunner runner;
    public MatchTimerUI matchTimerUI;

    public static AutoMatchManager Instance { get; private set; }
}
