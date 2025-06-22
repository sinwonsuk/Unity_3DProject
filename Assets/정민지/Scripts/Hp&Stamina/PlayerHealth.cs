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

    // ��Ʈ��ũ�� ����ȭ�� ü��
    [Networked] public int currentHp { get; private set; }
    [SerializeField] private int maxHp;

    public override void Spawned()
    {
        // ���� ������ ���� �� �ʱ� ü�� ����
        if (HasStateAuthority)
            currentHp = maxHp;

        // �Է� ������ �ִ� Ŭ���̾�Ʈ�� UI �ʱ�ȭ �̺�Ʈ ����
        if (Object.HasInputAuthority)
            EventBus<HealthChanged>.Raise(new HealthChanged(this, currentHp, maxHp));
    }

    /// <summary>
    /// ������� �����մϴ�.
    /// </summary>
    public void TakeDamage(int dmg)
    {
        // ������ ����
        if (!HasStateAuthority) return;

        int before = currentHp;
        currentHp = Mathf.Clamp(currentHp - dmg, 0, maxHp);
        Debug.Log($"[�÷��̾� ü��] {gameObject.name}: {before} �� {currentHp} (-{dmg})");

        // UI ������Ʈ
        if (Object.HasInputAuthority)
            EventBus<HealthChanged>.Raise(new HealthChanged(this, currentHp, maxHp));

        // ��� �� ������ �� ���
        if (currentHp <= 0)
            CountAlivePlayers();
    }

    /// <summary>
    /// ȸ�� ������ ���
    /// </summary>
    public void Heal(int amount)
    {
        if (!HasStateAuthority) return;

        int before = currentHp;
        currentHp = Mathf.Clamp(currentHp + amount, 0, maxHp);
        Debug.Log($"[�÷��̾� ü��] {gameObject.name}: {before} �� {currentHp} (+{amount})");

        if (Object.HasInputAuthority)
            EventBus<HealthChanged>.Raise(new HealthChanged(this, currentHp, maxHp));
    }

    /// <summary>
    /// ���� ������ �� ���� �� �̺�Ʈ ����
    /// </summary>
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

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void Rpc_RequestDamage(int damage) => TakeDamage(damage);

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void Rpc_RequestHeal(int heal) => Heal(heal);
}
