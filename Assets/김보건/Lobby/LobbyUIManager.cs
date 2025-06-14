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

    [Header("UI 텍스트 오브젝트들")]
    public TMP_Text nameText;       // Name 오브젝트
    public TMP_Text introduceText;  // introduce 오브젝트

    public MatchTimerUI matchTimerUI;

}
