using UnityEngine;

public class EscHandler : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BackToMenu();
        }
    }

    public void BackToMenu()
    {
        if (visibleUI != null && invisibleUI != null)
        {
            visibleUI.SetActive(false);
            invisibleUI.SetActive(true);
        }
    }

    public GameObject visibleUI;
    public GameObject invisibleUI;
}
