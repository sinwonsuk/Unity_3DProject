using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : NetworkBehaviour
{
    [Networked] public int currentHp { get; private set; }
    [SerializeField] private int maxHp;

    public override void Spawned()
    {
        if (HasStateAuthority)
            currentHp = maxHp;

        if (Object.HasInputAuthority)
        {
            EventBus<HealthChanged>.Raise(new HealthChanged(this, currentHp,maxHp));
        }
    }

    public void TakeDamage(int dmg)
    {
        if (!HasStateAuthority) return;

        currentHp = Mathf.Clamp(currentHp - dmg, 0, maxHp);

        Debug.Log($"플레이어 현재 체력 : {currentHp}");

        // 클라이언트 UI 업데이트
        if (Object.HasInputAuthority)
        {
            EventBus<HealthChanged>.Raise(new HealthChanged(this, currentHp, maxHp));
        }

        // 서버가 생존자 수 계산
        if (currentHp <= 0)
        {
            CountAlivePlayers(); // 이건 HasStateAuthority니까 안전하게 호출됨
        }
    }


    public void TakeDamages(int dmg)
    {
        if (!HasStateAuthority) return;

        currentHp = Mathf.Clamp(currentHp - dmg, 0, maxHp);

        Debug.Log($"플레이어 현재 체력 : {currentHp}");

        // 클라이언트 UI 업데이트
        if (Object.HasInputAuthority)
        {
            EventBus<HealthChanged>.Raise(new HealthChanged(this, currentHp, maxHp));
        }

        // 서버가 생존자 수 계산
        if (currentHp <= 0)
        {
            CountAlivePlayers(); // 이건 HasStateAuthority니까 안전하게 호출됨
        }
    }


    public void UseHealingItem(int heal)
    {

        if (!HasStateAuthority) return; //권한을 가지고있지 않으면 리턴

        currentHp = Mathf.Clamp(currentHp + heal, 0, maxHp);

        if (Object.HasInputAuthority)
        {
            EventBus<HealthChanged>.Raise(new HealthChanged(this, currentHp, maxHp));
        }
    }

    public void CountAlivePlayers()
    {
        int alive = 0;
        foreach (var player in Runner.ActivePlayers)
        {
            var obj = Runner.GetPlayerObject(player);
            if (obj != null && obj.TryGetComponent<PlayerHealth>(out var health))
            {
                if (health.currentHp > 0)
                    alive++;
            }
        }

        // 서버만 발행
        if (HasStateAuthority)
        {
            EventBus<SurvivorPlayerCount>.Raise(new SurvivorPlayerCount(alive));
        }
    }

    public void RequestDamage(int damage)
    {
        if (Object.HasInputAuthority)
            Rpc_RequestDamage(damage);
        else if(Object.HasStateAuthority)
            TakeDamage(damage);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void Rpc_RequestDamage(int damage)
    {
        TakeDamage(damage);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void Rpc_RequestHeal(int heal)
    {
        UseHealingItem(heal);
    }
}
