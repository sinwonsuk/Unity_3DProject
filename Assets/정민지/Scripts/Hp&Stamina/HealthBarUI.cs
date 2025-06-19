using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Image fillImage;
    public TMP_Text hpText;
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
        hpText.text = ($"{evt.currentHp} / {evt.maxHp}");
    }
}
