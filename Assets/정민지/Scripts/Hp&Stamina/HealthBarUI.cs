using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Collections.Unicode;

public class HealthBarUI : NetworkBehaviour
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
        if (evt.playerInfo.Object.InputAuthority != Runner.LocalPlayer) return;

        fillImage.fillAmount = (float)evt.currentHp / evt.maxHp;
        hpText.text = ($"{evt.currentHp} / {evt.maxHp}");
    }
}
