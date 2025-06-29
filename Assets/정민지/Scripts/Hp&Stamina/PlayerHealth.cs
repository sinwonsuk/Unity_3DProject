using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Addons.SimpleKCC;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : NetworkBehaviour
{

    public static readonly List<PlayerHealth> All = new List<PlayerHealth>();

    void Awake() => All.Add(this);
    void OnDestroy() => All.Remove(this);


    [Networked] public int currentHp { get; private set; }
    [Networked] public bool isDead { get; private set; }

    [SerializeField] private GameObject deadPrefab;
    [SerializeField] private int maxHp=100;
    [Networked] private int lastSentHp { get; set; } = -1;
    [Networked] private bool canWin { get; set; } = false;
    [SerializeField] private int imReady=0;
    [Networked] private float readyTimer { get; set; }

    private bool isSpectator = false;

    [SerializeField] private float timeBeforeHealing = 10f; // 10초 대기 시간
    [SerializeField] private float healInterval = 1f;       // 회복 주기 (1초)
    [SerializeField] private int healAmount = 2;            // 회복량

    [Networked] private float noDamageTimer { get; set; } = 0f;
    [Networked] private float healTimer { get; set; } = 0f;

    void Update()
    {

        ////테스트용
        //if (Object.HasInputAuthority && Input.GetKeyDown(KeyCode.P))
        //{
        //    // 로컬에서 관전 카메라 생성
        //    SpectatorManager.EnterSpectatorMode(transform.position, transform.rotation);

        //    RPC_RequestSuicide();
        //}

        // 관전모드진입
        if (Object.HasInputAuthority)
        {
            if(currentHp <= 0 && !isSpectator)
            {
                isSpectator = true;
                SpectatorManager.EnterSpectatorMode(transform.position, transform.rotation);
                //RPC_RequestSuicide();
                Instantiate(deadPrefab);
            }

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

        if (HasStateAuthority && !isDead)
        {
            // 데미지를 입지 않은 시간 증가
            if (currentHp < maxHp)
                noDamageTimer += Runner.DeltaTime;

            // 체력 회복
            if (noDamageTimer >= timeBeforeHealing)
            {
                healTimer += Runner.DeltaTime;
                if (healTimer >= healInterval)
                {
                    Heal(healAmount); // 회복
                    healTimer = 0f;
                }
            }

            if (readyTimer < imReady)
            {
                SurvivorManager.Instance?.UpdateSurvivorCount();
                readyTimer += Runner.DeltaTime;
            }
            else if (!canWin)
            {
                canWin = true;
            }
        }
    }

    //서버에게 HP 0 요청 
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestSuicide() => ApplyFatalDamage();

    //서버 전용 HP 처리, Despawn
    void ApplyFatalDamage()
    {
        if (!HasStateAuthority || isDead) return;

        isDead = true;                           //모든 클라로 복제
        SurvivorManager.Instance?.UpdateSurvivorCount();
        StartCoroutine(DelayDisableCharacter());

    }

    IEnumerator DelayDisableCharacter()
    {
        yield return new WaitForSeconds(1f);
        RPC_DisableCharacter();
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

        currentHp = Mathf.Clamp(currentHp - dmg, 0, maxHp);
        Debug.Log($"{gameObject.name} 체력 감소: {currentHp}");

        // 회복 타이머 초기화
        noDamageTimer = 0f;
        healTimer = 0f;

        if (currentHp <= 0 && !isDead)
        {
            isDead = true;
            SurvivorManager.Instance?.UpdateSurvivorCount();
        }
    }

    public void DelayedSurvivorUpdate()
    {
        SurvivorManager.Instance?.UpdateSurvivorCount();
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

        currentHp = Mathf.Clamp(currentHp + amount, 0, maxHp);
        Debug.Log($"{gameObject.name} 체력 감소: {currentHp}");

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


    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
    public void RPC_ShowWinUI()
    {
        EventBus<SurvivorWin>.Raise(new SurvivorWin());
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
    public void RPC_ShowLoseUI()
    {
        EventBus<SurvivorLose>.Raise(new SurvivorLose());
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_DisableCharacter()
    {
        //외형 , 충돌 끄기
        foreach (var r in GetComponentsInChildren<Renderer>(true)) r.enabled = false;
        foreach (var c in GetComponentsInChildren<Collider>(true)) c.enabled = false;

        //애니메이터 끄기
        foreach (var a in GetComponentsInChildren<Animator>(true)) a.enabled = false;
        var netAnim = GetComponent<NetworkMecanimAnimator>();
        if (netAnim) netAnim.enabled = false;

        //움직임 무기 스테이트머신 끄기
        DisableComponentByName("PlayerStateMachine");
        DisableComponentByName("WeaponManager");
        DisableComponentByName("SimpleKCC");
        DisableComponentByName("PlayerWeaponChanged");

        //모든 NetworkBehaviour 끄기 (PlayerHealth 제외)
        foreach (var nb in GetComponents<NetworkBehaviour>())
            if (nb != this) nb.enabled = false;

        //리지드바디 정지
        if (TryGetComponent<Rigidbody>(out var rb))
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
    }

    void DisableComponentByName(string componentTypeName)
    {
        var type = System.Type.GetType(componentTypeName);
        if (type == null) return;

        var comp = GetComponent(type) as UnityEngine.Behaviour;
        if (comp != null)
            comp.enabled = false;
    }
}