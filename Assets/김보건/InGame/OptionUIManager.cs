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

        // ���콺 ������ ���̰�
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void CloseOptions()
    {
        OptionsPanel.SetActive(false);
        isOptionsActive = false;

        // ���콺 ������ �ٽ� ����
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
        if (runner != null && runner.IsRunning && runner.IsServer)
        {
            Debug.Log("ȣ��Ʈ�� �����Ƿ� HostMigration ����");

                   // 1) ���� ����(������Ʈ ��ġ ��)�� ���������� ����
            var pushTask = runner.PushHostMigrationSnapshot();
            while (!pushTask.IsCompleted)
                yield return null;
            
                   // 2) HostMigration �÷���(=== true)�� �Ѱܼ� ����
            var shutdownTask = runner.Shutdown(true);
            while (!shutdownTask.IsCompleted)
                yield return null;

            RunnerSingleton.ClearRunner();
        }
        else if (runner != null)
        {
            // Ŭ���̾�Ʈ�� �׳� ������
            var shutdownTask = runner.Shutdown();
            while (!shutdownTask.IsCompleted) yield return null;

            RunnerSingleton.ClearRunner();
        }

        // Ÿ��Ʋ �� �ε��� �Ǵ� �̸����� �̵�
        SceneManager.LoadScene("TitleScene"); // �Ǵ� SceneManager.LoadScene(0);
    }



    public GameObject OptionsPanel;
    private bool isOptionsActive = false;
}
