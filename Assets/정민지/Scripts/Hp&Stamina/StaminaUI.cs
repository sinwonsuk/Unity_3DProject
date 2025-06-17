using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    [SerializeField] private Image staminaFill;
    [SerializeField] private TMP_Text staminaText;
    [SerializeField] private int maxStamina;
    [SerializeField] private int stamina;
    [SerializeField] private float recoveryTimer;
    private float currentFill;
    private float targetFill;
    [SerializeField] private float fillSmoothSpeed=3f;

    private void OnEnable()
    {
        EventBus<Stamina>.OnEvent += UpdateStamina;
        EventBus<UseStamina>.OnEvent += LoseStamina;
    }

    private void OnDisable()
    {
        EventBus<Stamina>.OnEvent -= UpdateStamina;
        EventBus<UseStamina>.OnEvent -= LoseStamina;
    }

    private void Start()
    {
        stamina = maxStamina; // �׻� �ִ� ���¹̳ʷ� ����
        currentFill = (float)stamina / maxStamina;
        staminaFill.fillAmount = currentFill;
        EventBus<Stamina>.Raise(new Stamina(stamina));
    }

    private void Update()
    {
        // 1�ʿ� 1�� ȸ��
        if (stamina <= maxStamina)
        {
            recoveryTimer += Time.deltaTime;

            if (recoveryTimer >= 1f)
            {
                stamina += 1;
                if (stamina > maxStamina)
                    stamina = maxStamina;

                EventBus<Stamina>.Raise(new Stamina(stamina));
                recoveryTimer = 0f;
            }
        }

        // ȸ���� �ε巴��, ���Ҵ� ���
        float targetFill = (float)stamina / maxStamina;

        if (currentFill < targetFill)
        {
            currentFill = Mathf.Lerp(currentFill, targetFill, fillSmoothSpeed * Time.deltaTime);
        }
        else
        {
            currentFill = targetFill;
        }

        staminaFill.fillAmount = currentFill;
    }


    private void UpdateStamina(Stamina stamina)
    {
        this.stamina = stamina.currentStamina;
       // staminaFill.fillAmount = this.stamina / maxStamina;
        staminaText.text = ($"{this.stamina} / {maxStamina}");
    }

    private void LoseStamina(UseStamina stamina)
    {
        if(this.stamina>stamina.useStamina)
        {
            this.stamina -= stamina.useStamina;
            EventBus<Stamina>.Raise(new Stamina(this.stamina));
        }
    }

    
}
