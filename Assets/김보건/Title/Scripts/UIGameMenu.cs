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

    public void ConfirmNicknameAndConnect()
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
        if (_runner == null)
        {
            Debug.LogError("RunnerSingleton에서 Runner를 찾을 수 없음");
            return;
        }

        DontDestroyOnLoad(_runner);
        _runner.ProvideInput = true;

        //씬 전환만 수행. StartGame은 AutoMatchManager에서 수행
        SceneManager.LoadScene("LobbyScene");
    }
}