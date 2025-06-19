using Fusion;
using UnityEngine;

public class WorldHealthBarController : NetworkBehaviour
{
    [SerializeField] private Canvas worldCanvas;

    public override void Spawned()
    {
        // 내 캐릭터이면 머리 위 체력바 비활성화
        if (Object.HasInputAuthority)
        {
            worldCanvas.enabled = false;
        }
    }
}
