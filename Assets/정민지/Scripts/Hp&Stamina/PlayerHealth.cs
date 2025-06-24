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

        Debug.Log($"�÷��̾� ���� ü�� : {currentHp}");

        // Ŭ���̾�Ʈ UI ������Ʈ
        if (Object.HasInputAuthority)
        {
            EventBus<HealthChanged>.Raise(new HealthChanged(this, currentHp, maxHp));
        }

        // ������ ������ �� ���
        if (currentHp <= 0)
        {
            CountAlivePlayers(); // �̰� HasStateAuthority�ϱ� �����ϰ� ȣ���
        }
    }


    public void TakeDamages(int dmg)
    {
        if (!HasStateAuthority) return;

        currentHp = Mathf.Clamp(currentHp - dmg, 0, maxHp);

        Debug.Log($"�÷��̾� ���� ü�� : {currentHp}");

        // Ŭ���̾�Ʈ UI ������Ʈ
        if (Object.HasInputAuthority)
        {
            EventBus<HealthChanged>.Raise(new HealthChanged(this, currentHp, maxHp));
        }

        // ������ ������ �� ���
        if (currentHp <= 0)
        {
            CountAlivePlayers(); // �̰� HasStateAuthority�ϱ� �����ϰ� ȣ���
        }
    }


    public void UseHealingItem(int heal)
    {

        if (!HasStateAuthority) return; //������ ���������� ������ ����

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

        // ������ ����
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
