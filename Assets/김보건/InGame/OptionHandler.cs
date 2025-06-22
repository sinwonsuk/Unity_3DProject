using UnityEngine;
using UnityEngine.Rendering.Universal;

public class OptionHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OptionsPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Esc Ű�� ������ �ɼ� �г��� ����մϴ�.
        if (Input.GetKeyDown(KeyCode.Escape) && isOption ==false)
        {
            OptionsPanel.SetActive(true);
            isOption = true;   
        }
        else if(Input.GetKeyDown(KeyCode.Escape) && isOption == true)
        {
            OptionsPanel.SetActive(false);
            isOption = false;
        }

    }

    public void BackToGame()
    {
        OptionsPanel.SetActive(false);
        isOption = false;
    }

    private bool isOption = false;
    public GameObject OptionsPanel;       // �ɼ� �г�
}
