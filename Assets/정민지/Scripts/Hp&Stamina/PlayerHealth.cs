using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : NetworkBehaviour
{
    // 모든 인스턴스를 추적하기 위한 리스트
    public static readonly List<PlayerHealth> All = new List<PlayerHealth>();

    void Awake() => All.Add(this);
    void OnDestroy() => All.Remove(this);

    // 네트워크로 동기화할 체력
    [Networked] public int currentHp { get; private set; }
    [SerializeField] private int maxHp;

    public override void Spawned()
    {
        // 서버 권한이 있을 때 초기 체력 설정
        if (HasStateAuthority)
            currentHp = maxHp;

        // 입력 권한이 있는 클라이언트에 UI 초기화 이벤트 발행
        if (Object.HasInputAuthority)
            EventBus<HealthChanged>.Raise(new HealthChanged(this, currentHp, maxHp));
    }

    /// <summary>
    /// 대미지를 적용합니다.
    /// </summary>
    public void TakeDamage(int dmg)
    {
        // 서버만 실행
        if (!HasStateAuthority) return;

        int before = currentHp;
        currentHp = Mathf.Clamp(currentHp - dmg, 0, maxHp);
        Debug.Log($"[플레이어 체력] {gameObject.name}: {before} → {currentHp} (-{dmg})");

        // UI 업데이트
        if (Object.HasInputAuthority)
            EventBus<HealthChanged>.Raise(new HealthChanged(this, currentHp, maxHp));

        // 사망 시 생존자 수 계산
        if (currentHp <= 0)
            CountAlivePlayers();
    }

    /// <summary>
    /// 회복 아이템 사용
    /// </summary>
    public void Heal(int amount)
    {
        if (!HasStateAuthority) return;

        int before = currentHp;
        currentHp = Mathf.Clamp(currentHp + amount, 0, maxHp);
        Debug.Log($"[플레이어 체력] {gameObject.name}: {before} → {currentHp} (+{amount})");

        if (Object.HasInputAuthority)
            EventBus<HealthChanged>.Raise(new HealthChanged(this, currentHp, maxHp));
    }

    /// <summary>
    /// 현재 생존자 수 집계 및 이벤트 발행
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
