using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    [SerializeField] private Image staminaFill;
    [SerializeField] private TMP_Text staminaText;

    private void OnEnable()
    {
        EventBus<StaminaChanged>.OnEvent += UpdateStamina;
    }

    private void OnDisable()
    {
        EventBus<StaminaChanged>.OnEvent -= UpdateStamina;
    }

    private void UpdateStamina(StaminaChanged evt)
    {
        if(!evt._playerInfo.Object.HasInputAuthority) return;

        staminaFill.fillAmount = (float)evt.currentStamina / evt.maxStamina;
        staminaText.text = ($"{evt.currentStamina} / {evt.maxStamina}");
        Debug.Log("스테미나 게이지 갱신");
    }
}