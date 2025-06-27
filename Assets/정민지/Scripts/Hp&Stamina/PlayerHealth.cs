using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : NetworkBehaviour
{

    public static readonly List<PlayerHealth> All = new List<PlayerHealth>();

    void Awake() => All.Add(this);
    void OnDestroy() => All.Remove(this);


    [Networked] public int currentHp { get; private set; }
    [SerializeField] private int maxHp;
    private int lastSentHp = -1;

    void Update()
    {
        if (Object.HasInputAuthority && Input.GetKeyDown(KeyCode.P))
        {
            // 로컬에서 관전 카메라 생성 (RPC X)
            SpectatorManager.EnterSpectatorMode(transform.position, transform.rotation);

            // 자살 요청 RPC
            RPC_RequestSuicide();
        }
    }

    public override void FixedUpdateNetwork()
    {
        // 본인의 캐릭터라면
        if (Object.InputAuthority == Runner.LocalPlayer)
        {
            if (lastSentHp != currentHp)
            {
                lastSentHp = currentHp;
                EventBus<HealthChanged>.Raise(new HealthChanged(this, currentHp, maxHp));
            }
        }
    }

    //서버에게 HP 0 요청 
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    void RPC_RequestSuicide() => ApplyFatalDamage();

    //서버 전용 HP 처리, Despawn
    void ApplyFatalDamage()
    {
        if (!HasStateAuthority) return;

        currentHp = 0;
        Runner.Despawn(Object);           // 캐릭터 제거
    }

    public override void Spawned()
    {

        if (HasStateAuthority)
            currentHp = maxHp;


        if (Object.HasInputAuthority)
            EventBus<HealthChanged>.Raise(new HealthChanged(this, currentHp, maxHp));


    }



    public void TakeDamage(int dmg)
    {
        
        if (!HasStateAuthority) return;

        int before = currentHp;
        currentHp = Mathf.Clamp(currentHp - dmg, 0, maxHp);
        Debug.Log($"{gameObject.name}: {before} > {currentHp} (-{dmg})");

        //if (Object.InputAuthority == Runner.LocalPlayer)
        //    EventBus<HealthChanged>.Raise(new HealthChanged(this, currentHp, maxHp));

        if (currentHp <= 0)
        {
            SurvivorManager.Instance?.UpdateSurvivorCount();
        }
           // CountAlivePlayers();
    }
    public void TakeDamages(int dmg)
    {
        if (!HasStateAuthority) return;

        currentHp = Mathf.Clamp(currentHp - dmg, 0, maxHp);

        Debug.Log($"현재 체력: {currentHp}");

        if (Object.HasInputAuthority)
        {
            EventBus<HealthChanged>.Raise(new HealthChanged(this, currentHp, maxHp));
        }

        // ������ ������ �� ���
        if (currentHp <= 0)
        {
            CountAlivePlayers(); 
        }
    }
    public void Heal(int amount)
    {
        if (!HasStateAuthority) return;

        int before = currentHp;
        currentHp = Mathf.Clamp(currentHp + amount, 0, maxHp);
        Debug.Log($"{gameObject.name}: {before} > {currentHp} (+{amount})");

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
        else if (Object.HasStateAuthority)
            TakeDamage(damage);
    }

    public void RestoreHealth(int value)
    {
        if (HasStateAuthority)
            currentHp = Mathf.Clamp(value, 0, maxHp);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void Rpc_RequestDamage(int damage) => TakeDamage(damage);

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void Rpc_RequestHeal(int heal) => Heal(heal);
}