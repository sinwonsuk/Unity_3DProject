using UnityEngine;
using UnityEngine.SceneManagement;

public class SurvivorResultUI : MonoBehaviour
{
    public GameObject panelObject;     // Panel 오브젝트
    public GameObject winUI;
    public GameObject loseUI;

    private bool alreadyShown = false;

    void OnEnable()
    {
        EventBus<SurvivorWin>.OnEvent += ShowWin;
        EventBus<SurvivorLose>.OnEvent += ShowLose;
    }

    void OnDisable()
    {
        EventBus<SurvivorWin>.OnEvent -= ShowWin;
        EventBus<SurvivorLose>.OnEvent -= ShowLose;
    }

    void Update()
    {
        if (!alreadyShown) return;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            StartCoroutine(ReturnToTitle());
        }
    }

    void ShowWin(SurvivorWin e)
    {
        if (alreadyShown) return;
        alreadyShown = true;

        panelObject.SetActive(true);
        winUI.SetActive(true);
        loseUI.SetActive(false);
    }

    void ShowLose(SurvivorLose e)
    {
        if (alreadyShown) return;
        alreadyShown = true;

        panelObject.SetActive(true);
        winUI.SetActive(false);
        loseUI.SetActive(true);
    }

    private System.Collections.IEnumerator ReturnToTitle()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        var runner = RunnerSingleton.Instance;

        if (runner != null && runner.IsRunning)
        {
            if (BasicSpawner2.Instance != null)
            {
                BasicSpawner2.Instance.DespawnSelf();
            }

            var shutdownTask = runner.Shutdown();
            while (!shutdownTask.IsCompleted)
                yield return null;

            RunnerSingleton.ClearRunner();
        }

        SceneManager.LoadScene("TitleScene");
    }
}
