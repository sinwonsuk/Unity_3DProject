using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : NetworkBehaviour
{
    // ��� �ν��Ͻ��� �����ϱ� ���� ����Ʈ
    public static readonly List<PlayerHealth> All = new List<PlayerHealth>();

    void Awake() => All.Add(this);
    void OnDestroy() => All.Remove(this);


    [Networked] public int currentHp { get; private set; }
    [SerializeField] private int maxHp;

    public override void Spawned()
    {

        if (HasStateAuthority)
            currentHp = maxHp;


        if (Object.HasInputAuthority)
            EventBus<HealthChanged>.Raise(new HealthChanged(this, currentHp, maxHp));

        InventoryUI.Instance.SelectFirstWeaponSlot();

    }

    //public override void FixedUpdateNetwork()
    //{
    //    InventoryUI.Instance.IsOpenInven();
    //}

    public void TakeDamage(int dmg)
    {
        
        if (!HasStateAuthority) return;

        int before = currentHp;
        currentHp = Mathf.Clamp(currentHp - dmg, 0, maxHp);
        Debug.Log($"[�÷��̾� ü��] {gameObject.name}: {before} �� {currentHp} (-{dmg})");

        if (Object.HasInputAuthority)
            EventBus<HealthChanged>.Raise(new HealthChanged(this, currentHp, maxHp));

        if (currentHp <= 0)
            CountAlivePlayers();
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
    public void Heal(int amount)
    {
        if (!HasStateAuthority) return;

        int before = currentHp;
        currentHp = Mathf.Clamp(currentHp + amount, 0, maxHp);
        Debug.Log($"[�÷��̾� ü��] {gameObject.name}: {before} �� {currentHp} (+{amount})");

        if (Object.HasInputAuthority)
            EventBus<HealthChanged>.Raise(new HealthChanged(this, currentHp, maxHp));
    }

    public void CountAlivePlayers()
    {
        int alive = 0;
        foreach (var player in All)
        {
            if (player.currentHp > 0)
                alive++;
        }
        if (HasStateAuthority)
            EventBus<SurvivorPlayerCount>.Raise(new SurvivorPlayerCount(alive));
    }

    public void RequestDamage(int damage)
    {
        if (Object.HasInputAuthority)
            Rpc_RequestDamage(damage);
        else if(Object.HasStateAuthority)
            TakeDamage(damage);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void Rpc_RequestDamage(int damage) => TakeDamage(damage);

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void Rpc_RequestHeal(int heal) => Heal(heal);
}
