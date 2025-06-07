using UnityEngine;

public class LoginUI : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void OnClickLogin()
    {
        gameObject.SetActive(true);
    }

    private void Update()
    {
        if(gameObject.activeSelf==true&&Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(false);
        }
    }
}
