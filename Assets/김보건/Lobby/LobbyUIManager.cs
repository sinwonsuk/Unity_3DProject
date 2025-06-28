using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyUIManager : MonoBehaviour
{
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        zoomUI.SetActive(false);
    }

    public void ShowCharacterInfo(string name, string desc, string nick)
    {
        zoomUI.SetActive(true);
        BackButton.SetActive(false);

        nickText.text = nick;
        nameText.text = name;
        introduceText.text = desc;
    }

    public void HideZoomUI()
    {
        zoomUI.SetActive(false);
        BackButton.SetActive(true);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("TitleScene");
    }

    public static LobbyUIManager Instance;

    public GameObject zoomUI;
    public GameObject BackButton;

    [Header("UI �ؽ�Ʈ ������Ʈ��")]
    public TMP_Text nickText;
    public TMP_Text nameText;       // Name ������Ʈ
    public TMP_Text introduceText;  // introduce ������Ʈ

    public MatchTimerUI matchTimerUI;
    public GameObject chooseCharacterUI;

}
