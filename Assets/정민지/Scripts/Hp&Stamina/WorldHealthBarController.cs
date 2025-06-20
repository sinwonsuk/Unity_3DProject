using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class WorldHealthBarController : NetworkBehaviour
{
    [SerializeField] private Canvas worldCanvas;
    [SerializeField] private Image hpFill;

    public override void Spawned()
    {
        // 내 캐릭터이면 머리 위 체력바 비활성화
        if (Object.HasInputAuthority)
        {
            worldCanvas.enabled = false;
        }
    }

    private void OnEnable()
    {
        EventBus<HealthChanged>.OnEvent += ShowCurrentHP;
    }

    private void OnDisable()
    {
        EventBus<HealthChanged>.OnEvent -= ShowCurrentHP;
    }

    public void ShowCurrentHP(HealthChanged evt)
    {
        if (!evt.playerInfo.Object.HasInputAuthority) return;

        hpFill.fillAmount = (float)evt.currentHp / evt.maxHp;
    }
}
