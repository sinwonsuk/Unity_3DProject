using Fusion;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionUIManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OptionsPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isOptionsActive)
        {
            OpenOptions();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && isOptionsActive)
        {
            CloseOptions();
        }
    }

    public void BackToGame()
    {
        if (isOptionsActive)
        {
            CloseOptions();
        }
    }

    private void OpenOptions()
    {
        OptionsPanel.SetActive(true);
        isOptionsActive = true;

        // 마우스 포인터 보이게
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void CloseOptions()
    {
        OptionsPanel.SetActive(false);
        isOptionsActive = false;

        // 마우스 포인터 다시 숨김
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void EscapeGame()
    {
        StartCoroutine(ReturnToTitle());
    }

    private IEnumerator ReturnToTitle()
    {
        var runner = RunnerSingleton.Instance;

        if (runner != null && runner.IsRunning)
        {
            // 내 캐릭터 제거
            if (BasicSpawner2.Instance != null)
            {
                BasicSpawner2.Instance.DespawnSelf();
            }

            var shutdownTask = runner.Shutdown();
            while (!shutdownTask.IsCompleted)
                yield return null;

            RunnerSingleton.ClearRunner();
        }

        // 타이틀 씬으로 이동
        SceneManager.LoadScene("TitleScene");
    }



    public GameObject OptionsPanel;
    private bool isOptionsActive = false;
}
