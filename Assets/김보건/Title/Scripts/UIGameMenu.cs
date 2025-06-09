using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIGameMenu : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject StartGamePanel;     // STARTGAME 버튼
    public GameObject NicknamePanel;      // 닉네임 입력 
    public GameObject OptionsPanel;       // 옵션 패널
    public GameObject ExitPanel;          // 종료 확인 패널

    [Header("Nickname UI")]
    public TMP_InputField NicknameText;

    [Header("Network")]
    public NetworkRunner RunnerPrefab;    // NetworkRunner 프리팹

    private NetworkRunner _runner;


    private void Start()
    {
        StartGamePanel.SetActive(true);
        NicknamePanel.SetActive(false);

        string savedName = PlayerPrefs.GetString("PlayerName", "");
        if (!string.IsNullOrEmpty(savedName))
        {
            NicknameText.text = savedName;
        }
        else
        {
            NicknameText.text = "Player" + Random.Range(1000, 9999);
        }
    }

    public void ShowNicknamePanel()
    {
        StartGamePanel.SetActive(false);
        NicknamePanel.SetActive(true);
        OptionsPanel.SetActive(false);
        ExitPanel.SetActive(false);
    }

    public void ShowOptionsPanel()
    {
        StartGamePanel.SetActive(false);
        OptionsPanel.SetActive(true);
        NicknamePanel.SetActive(false);
        ExitPanel.SetActive(false);
    }

    public void ShowExitPanel()
    {
        StartGamePanel.SetActive(false);
        ExitPanel.SetActive(true);
        NicknamePanel.SetActive(false);
        OptionsPanel.SetActive(false);
    }

    public void ConfirmExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public async void ConfirmNicknameAndConnect()
    {
        string nickname = NicknameText.text;
        if (string.IsNullOrWhiteSpace(nickname))
        {
            Debug.LogWarning("닉네임을 입력하세요");
            return;
        }

        // 닉네임 저장
        PlayerPrefs.SetString("PlayerName", nickname);
        PlayerPrefs.Save();

        Debug.Log("닉네임 저장: " + nickname);

        // Runner 생성 및 설정
        _runner = RunnerSingleton.Instance;
        _runner.ProvideInput = true;
        _runner.name = "NetworkRunner";
        DontDestroyOnLoad(_runner);
        _runner.ProvideInput = true;

        var result = await _runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = "DefaultRoom",
            Scene = SceneRef.FromIndex(1),
            SceneManager = _runner.GetComponent<NetworkSceneManagerDefault>()
        });

        if (result.Ok)
        {
            Debug.Log("서버 연결 성공 및 씬 전환 완");
        }
        else
        {
            Debug.LogError($"서버 연결 실패: {result.ShutdownReason}");
        }
    }
}