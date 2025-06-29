// PlayerStamina.cs
using Fusion;
using UnityEngine;

public class PlayerStamina : NetworkBehaviour
{
    // 1) 네트워크드 프로퍼티 — 호스트가 결정하고 전파합니다
    [Networked] public float currentStamina { get; set; }
    [Networked] public float maxStamina { get; set; }

    // 회복/쿨다운 로직은 로컬 클라이언트에서만 필요하다면 Update()에서 처리하세요.
    private float recoveryTimer;
    private bool isRecovering;
    [SerializeField] private int staminaCoolTime = 3;

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            // 호스트가 한 번만 초기화
            maxStamina = 100f;
            currentStamina = maxStamina;
        }
    }



    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    void RPC_AttackStamina(float amount)
    {
        currentStamina = Mathf.Clamp(currentStamina - amount, 0f, maxStamina);
        RPC_Sound();
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    void RPC_Stamina(float amount)
    {
        currentStamina = Mathf.Clamp(currentStamina - amount, 0f, maxStamina);
        EventBus<StaminaChanged>.Raise(new StaminaChanged((float)currentStamina, (int)maxStamina));
    }


    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
    void RPC_Sound()
    {
        if(!Object.HasInputAuthority)
            return;

        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Sword,false);
    }

    // 2) 스태미나 사용 메소드
    public void UseStamina(float amount)
    {
        if(Object.HasInputAuthority)
        {
            RPC_AttackStamina(amount);
        }
        else if(Object.HasStateAuthority)
        {
            currentStamina = Mathf.Clamp(currentStamina - amount, 0f, maxStamina);
        }
    }

    public void ConsumeStaminaOnServer(float amount)
    {
        if (Object.HasStateAuthority)
        {
            currentStamina = Mathf.Clamp(currentStamina - amount, 0f, maxStamina);
        }
        else if(Object.HasInputAuthority)
        {
            RPC_Stamina(amount);
        }
    }

    public void AttackStaminaOnServer(float amount)
    {
        if (Object.HasStateAuthority)
        {
            currentStamina = Mathf.Clamp(currentStamina - amount, 0f, maxStamina);
            RPC_Sound();
        }
        else if (Object.HasInputAuthority)
        {
            RPC_AttackStamina(amount);
        }
    }


    // 3) (선택) 클라이언트 UI / 디버그용
    public override void FixedUpdateNetwork()
    {
        // 로컬 클라이언트만 스태미나 회복 연산을 하고 싶다면
        if (Object.HasStateAuthority)
        {
            if (!isRecovering && currentStamina < maxStamina)
            {
                recoveryTimer += Runner.DeltaTime;
                if (recoveryTimer >= 1f)
                {
                    recoveryTimer = 0f;
                    currentStamina = Mathf.Clamp(currentStamina + 5f, 0f, maxStamina);
                }
            }

            if (!isRecovering && currentStamina <= 1f)
            {
                isRecovering = true;
                recoveryTimer = 0f;
            }
            else if (isRecovering)
            {
                recoveryTimer += Runner.DeltaTime;
                if (recoveryTimer >= staminaCoolTime)
                {
                    currentStamina = 2f;
                    isRecovering = false;
                    recoveryTimer = 0f;
                }
            }

            // 디버그
            Debug.Log($"[Player {Object.InputAuthority.PlayerId}] Stamina = {currentStamina:F1}/{maxStamina}");
        }


    }
}