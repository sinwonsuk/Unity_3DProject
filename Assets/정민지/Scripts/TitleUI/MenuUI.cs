using UnityEngine;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private GameObject login;
    [SerializeField] private GameObject option;
    [SerializeField] private GameObject exit;

    void Start()
    {
        login.gameObject.SetActive(false);
        option.gameObject.SetActive(false);
        exit.gameObject.SetActive(false);
    }

    public void OnClickLogin()
    {
        login.gameObject.SetActive(true);
    }

    public void OnClickOption()
    {
        option.gameObject.SetActive(true);
    }

    public void OnClickExit()
    {
        exit.gameObject.SetActive(true);
    }

    void Update()
    {
        if(login.gameObject.activeSelf==true&&Input.GetKeyDown(KeyCode.Escape))
        {
            login.gameObject.SetActive(false);
        }

        if (option.gameObject.activeSelf == true && Input.GetKeyDown(KeyCode.Escape))
        {
            option.gameObject.SetActive(false);
        }

        if (exit.gameObject.activeSelf == true && Input.GetKeyDown(KeyCode.Escape))
        {
            exit.gameObject.SetActive(false);
        }
    }
}
