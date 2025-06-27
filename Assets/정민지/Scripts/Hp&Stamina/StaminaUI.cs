using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    private float lastStaminaValue = -1f;

    [SerializeField] private Image staminaFill;
    [SerializeField] private TMP_Text staminaText;

    private void OnEnable()
    {
        EventBus<StaminaChanged>.OnEvent += OnStaminaChanged;
    }

    private void OnDisable()
    {
        EventBus<StaminaChanged>.OnEvent -= OnStaminaChanged;
    }

    public void OnStaminaChanged(StaminaChanged e)
    {
        float normalized = (float)e.currentStamina / e.maxStamina;

        if (Mathf.Abs(normalized - lastStaminaValue) > 0.001f)
        {
            staminaFill.fillAmount = normalized;
            lastStaminaValue = normalized;

            // ���¹̳� ��ġ �ؽ�Ʈ ǥ��
            staminaText.text = $"{e.currentStamina:0}/{e.maxStamina}";
        }
    }
}