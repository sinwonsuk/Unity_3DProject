using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Image fillImage;
    public PlayerRef myPlayerRef;

    void OnEnable()
    {
        EventBus<HealthChanged>.OnEvent += OnHealthChanged;
    }

    void OnDisable()
    {
        EventBus<HealthChanged>.OnEvent -= OnHealthChanged;
    }

    void OnHealthChanged(HealthChanged evt)
    {
        if (evt.player != myPlayerRef) return;

        fillImage.fillAmount = (float)evt.currentHp / evt.maxHp;
    }
}
