using TMPro;
using UnityEngine;

public class LobbyUIManager : MonoBehaviour
{
    private void Awake()
    {
        Instance = this;
    }

    public void ShowCharacterInfo(string name, string desc)
    {
        zoomUI.SetActive(true);

        nameText.text = name;
        introduceText.text = desc;
    }

    public void HideZoomUI()
    {
        zoomUI.SetActive(false);
    }

    public static LobbyUIManager Instance;

    public GameObject zoomUI;

    [Header("UI �ؽ�Ʈ ������Ʈ��")]
    public TMP_Text nameText;       // Name ������Ʈ
    public TMP_Text introduceText;  // introduce ������Ʈ

    public MatchTimerUI matchTimerUI;

}
