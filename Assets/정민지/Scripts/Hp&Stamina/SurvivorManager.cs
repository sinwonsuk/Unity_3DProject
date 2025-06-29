using Fusion;
using UnityEngine;

public class SurvivorManager : NetworkBehaviour
{
    public static SurvivorManager Instance { get; private set; }

    public override void Spawned()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void UpdateSurvivorCount()
    {
        if (!HasStateAuthority) return;

        int alive = 0;

        PlayerHealth lastSurvivor = null;

        foreach (var player in PlayerHealth.All)
        {
            if (player.currentHp > 0 || !player.isDead)
            {
                alive++;
                lastSurvivor = player;
            }
        }

        RpcBroadcastSurvivorCount(alive);

        if (alive == 1 && lastSurvivor != null)
        {
            SurvivorResultManager.Instance?.DistributeResult(lastSurvivor);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RpcBroadcastSurvivorCount(int count)
    {
        Debug.Log($"[SurvivorManager] Received survivor count: {count}");
        EventBus<SurvivorPlayerCount>.Raise(new SurvivorPlayerCount(count));
    }
}