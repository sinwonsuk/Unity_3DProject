using Fusion;
using System.Collections;
using UnityEngine;

public class SurvivorManager : NetworkBehaviour
{
    public static SurvivorManager Instance { get; private set; }

    private float gameStartTime;
    private bool canEvaluateResult = false;

    public override void Spawned()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (HasStateAuthority)
        {
            gameStartTime = Time.time;
            StartCoroutine(EnableResultEvaluationAfterDelay(20f));
        }
    }

    private IEnumerator EnableResultEvaluationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canEvaluateResult = true;
        Debug.Log("[SurvivorManager] 승패 판단 시작됨 (20초 경과)");
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

        if (!canEvaluateResult)
            return; // 20초 전엔 판단 안 함

        if (alive == 1 && lastSurvivor != null)
        {
            SurvivorResultManager.Instance?.DistributeResult(lastSurvivor);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RpcBroadcastSurvivorCount(int count)
    {
        //Debug.Log($"[SurvivorManager] Received survivor count: {count}");
        EventBus<SurvivorPlayerCount>.Raise(new SurvivorPlayerCount(count));
    }
}