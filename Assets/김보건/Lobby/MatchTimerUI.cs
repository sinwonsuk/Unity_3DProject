using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MatchTimerUI : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private Button startButton;

    private Coroutine timerCoroutine;
    private float elapsedTime;

    public void StartTimer()
    {
        if (timerCoroutine != null) return;

        elapsedTime = 0f;
        timerCoroutine = StartCoroutine(UpdateTimer());
        if (startButton != null)
            startButton.interactable = false;
    }

    public void StopTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }
    }

    private IEnumerator UpdateTimer()
    {
        while (true)
        {
            elapsedTime += Time.deltaTime;
            int minutes = Mathf.FloorToInt(elapsedTime / 60f);
            int seconds = Mathf.FloorToInt(elapsedTime % 60f);

            timerText.text = $"{minutes:D2}:{seconds:D2}";
            yield return null;
        }
    }

    public void ResetTimer()
    {
        StopTimer();
        elapsedTime = 0f;
        timerText.text = "시작하기";
        if (startButton != null)
            startButton.interactable = true;
    }
}