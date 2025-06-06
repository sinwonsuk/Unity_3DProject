using UnityEngine;

public class LobbyUIManager : MonoBehaviour
{
    public GameObject matchingUI;         
    public GameObject characterSelectUI; 

    private void Start()
    {
        matchingUI.SetActive(true);
        characterSelectUI.SetActive(false);
    }

    public void OnClick_MatchingButton()
    {
        matchingUI.SetActive(false);   
        characterSelectUI.SetActive(true);     
    }
}
