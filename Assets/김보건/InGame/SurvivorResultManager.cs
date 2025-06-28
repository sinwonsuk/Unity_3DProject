using Fusion;
using UnityEngine;

public class SurvivorResultManager : NetworkBehaviour
{
    public static SurvivorResultManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void DistributeResult(PlayerHealth winner)
    {
        if (!HasStateAuthority) return;

        foreach (var player in PlayerHealth.All)
        {
            if (player == winner)
                player.RPC_ShowWinUI();
            else
                player.RPC_ShowLoseUI();
        }
    }
}
