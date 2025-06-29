using UnityEngine;

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
}
