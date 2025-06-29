using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Collections;
using static Unity.Collections.Unicode;
using Unity.VisualScripting.Antlr3.Runtime;

public class AutoMatchManager : MonoBehaviour, INetworkRunnerCallbacks
{

    private void Awake()
    {
        Instance = this;
    }

    //public void OnMatchButtonClick()
    //{
    //    if (RunnerSingleton.Instance != null && RunnerSingleton.Instance.IsRunning && !isMatching)
    //    { 
    //        Debug.LogWarning("러너 종료 중 - 재매칭은 잠시 뒤에 가능합니다"); 
    //        return; 
    //    }

    //    if (RunnerSingleton.Instance != null && !RunnerSingleton.Instance.IsRunning)
    //        RunnerSingleton.ClearRunner();

    //    matchTimerUI?.StartTimer();

    //    isMatching = true;

    //    if (RunnerSingleton.Instance == null)
    //        RunnerSingleton.CreateRunner();

    //    runner = RunnerSingleton.Instance;

    //    runner.ProvideInput = true;
    //    runner.AddCallbacks(this);

    //    string roomName = MatchQueueManager.Instance.CurrentRoomName;

    //    if (string.IsNullOrEmpty(roomName))
    //    {
    //        roomName = $"Room_{Random.Range(1000, 9999)}";
    //        MatchQueueManager.Instance.CurrentRoomName = roomName;
    //        Debug.Log($"방 생성 및 접속: {roomName}");
    //    }
    //    else
    //    {
    //        Debug.Log($"참가자 기존 방 입장: {roomName}");
    //    }

    //    StartGameWithRoomName(roomName);
    //}
    public void Start()
    {
        SoundManager.GetInstance().Bgm_Stop();
        SoundManager.GetInstance().PlayBgm(SoundManager.bgm.Lobby);
    }
    public void OnMatchButtonClick()
    {
        // 아직 매칭이 끝나지 않았거나 Runner가 종료 중이면 클릭 무시
        if (isMatching || shuttingDown) return;

        if (matchTimerUI != null)
        {
            matchTimerUI.StartTimer();
            SoundManager.GetInstance().Bgm_Stop();
            SoundManager.GetInstance().PlayBgm(SoundManager.bgm.Matching);
        }

        StartCoroutine(StartMatchFlow());
    }


    private IEnumerator StartMatchFlow()
    {
        //Runner가 살아 있다면 완전히 내려갈 때까지 대기
        if (RunnerSingleton.Instance != null)
        {
            shuttingDown = true;

            var task = RunnerSingleton.Instance.Shutdown(true);
            while (!task.IsCompleted)          // Disconnected 상태까지 기다림
                yield return null;

            RunnerSingleton.ClearRunner();
            shuttingDown = false;
        }

        //새 Runner 생성 후 처음부터 매칭 시작
        RunnerSingleton.CreateRunner();
        runner = RunnerSingleton.Instance;

        runner.ProvideInput = true;
        runner.AddCallbacks(this);

        var lobbyResult = runner.JoinSessionLobby(SessionLobby.ClientServer);
        while (!lobbyResult.IsCompleted) 
            yield return null;

        if (!lobbyResult.Result.Ok)
        {
            Debug.LogError("로비 참가 실패");
            yield break;
        }

        // 세션 리스트 대기 후 확인
        sessionListReceived = false;
        float timeout = 3f;
        while (!sessionListReceived && timeout > 0f)
        {
            timeout -= Time.deltaTime;
            yield return null;
        }

        string roomName = "";

        if (cachedSessionList.Count > 0)
        {
            //roomName = cachedSessionList[0].Name;
            //roomName = $"Room_{Random.Range(0, 9999)}";
            roomName = $"Room_0023";
            Debug.Log($"기존 방 참가: {roomName}");
        }
        else
        {
            //roomName = $"Room_{Random.Range(0, 9999)}";
            roomName = $"Room_0023";
            Debug.Log($"새 방 생성: {roomName}");
        }

        MatchQueueManager.Instance.CurrentRoomName = roomName;

        isMatching = true;
        StartGameWithRoomName(roomName);
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
        var startArgs = new StartGameArgs
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = roomName,
            Scene = startScene,         // Host 가 나중에 LoadScene
            SceneManager = sceneManager,
            PlayerCount = 3

        };

        try
        {
            StartGameResult result = await runner.StartGame(startArgs);

            if (result.Ok)
            {
                Debug.Log("StartGame 성공");
            }
            else
            {
                Debug.LogError($"StartGame 실패 ▶ {result.ShutdownReason}  Msg={result.ErrorMessage}");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"StartGame 중 예외 발생: {ex.Message}\n{ex.StackTrace}");
        }
    }

    public async void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"플레이어 입장: {player}. 현재 인원: {runner.ActivePlayers.Count()}");

        if (runner.IsServer && runner.ActivePlayers.Count() == 3)
        {
            Debug.Log("인게임 씬으로 이동 시작");

            // 매칭 완료되면 현재 방 이름 초기화
            MatchQueueManager.Instance.CurrentRoomName = "";

            await runner.LoadScene(SceneRef.FromIndex(2), LoadSceneMode.Single);
        }
    }
    public void CancelMatch()
    {
        if (!isMatching) return;

        isMatching = false;

        if (runner != null && runner.IsRunning)
        {
            Debug.Log("매칭 취소 중");
            // 러너 내부 정리가 끝날 때까지 기다렸다가 리셋
            StartCoroutine(ShutdownAndReset());
            SoundManager.GetInstance().Bgm_Stop();
            SoundManager.GetInstance().PlayBgm(SoundManager.bgm.Lobby);
        }
        else
        {
            // 러너가 이미 죽어있다면 UI만 리셋
            ResetLobbyState();
        }
    }

    private IEnumerator ShutdownAndReset()
    {
        var shutdownTask = runner.Shutdown(true); // 셧다운 완료까지 대기
        while (!shutdownTask.IsCompleted)
            yield return null;                    // 한 프레임씩 대기

        ResetLobbyState();
    }

    private void ResetLobbyState()
    {
        matchTimerUI.ResetTimer();                // 버튼/타이머 UI 정상화
        MatchQueueManager.Instance.CurrentRoomName = null;

        RunnerSingleton.ClearRunner();            // 러너 파괴 후 초기화
        runner = null;                           
    }


    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        cachedSessionList = sessionList;
        sessionListReceived = true;

        Debug.Log($"세션 리스트 수신됨. 방 수: {cachedSessionList.Count}");
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnConnectedToServer(NetworkRunner runner) { Debug.Log("서버에 연결 성공"); }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) {
        Debug.Log($"Shutdown 완료, Reason = {shutdownReason}");
    }
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

    private void OnMigrationResume(NetworkRunner runnerB)
    {
    }

    private NetworkRunner runner;
    public MatchTimerUI matchTimerUI;
    private bool isMatching = false;
    private bool shuttingDown = false;

    private List<SessionInfo> cachedSessionList = new List<SessionInfo>();
    private bool sessionListReceived = false;

    public static AutoMatchManager Instance { get; private set; }
    public bool IsMatching => isMatching;
}
