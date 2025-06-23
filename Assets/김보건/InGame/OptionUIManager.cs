using UnityEngine;

public class OptionUIManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OptionsPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && isOptionsActive == false)
        {
            OptionsPanel.SetActive(true);
            isOptionsActive = true;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && isOptionsActive == true)
        {
            OptionsPanel.SetActive(false);
            isOptionsActive = false;
        }
    }

    public void BackToGame()
    {
        if(isOptionsActive == true)
        {
            OptionsPanel.SetActive(false);
            isOptionsActive = false;
        }
    }



    public GameObject OptionsPanel;
    private bool isOptionsActive = false;
}
