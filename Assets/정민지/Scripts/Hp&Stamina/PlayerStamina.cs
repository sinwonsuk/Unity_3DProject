using Fusion;
using UnityEngine;

public class PlayerStamina : NetworkBehaviour
{
    [Networked] public float currentStamina { get; set; }

    public float AttackStaminaCost { get; set; } = 10f;

    public float MagicStaminaCost { get; set; } = 10f;

    public float ArrowStaminaCost { get; set; } = 10f;

    [SerializeField] private int maxStamina = 100;
    [SerializeField] private float recoveryTimer;
    public bool IsStamania { get; set; } = false;
    private bool imRunning = false;
    private float timer = 0f;
    private float skillTimer;
    private bool isRecovering = false;

    [SerializeField] private int staminaCoolTime;

    private void OnEnable()
    {
        EventBus<isRunning>.OnEvent += OnOffRunning;
    }

    private void OnDisable()
    {
        EventBus<isRunning>.OnEvent -= OnOffRunning;
    }
    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            currentStamina = maxStamina;
            EventBus<StaminaChanged>.Raise(new StaminaChanged(this, currentStamina, maxStamina));
        }
    }

    private void OnOffRunning(isRunning evt)
    {
        imRunning = evt.imRunning;
    }


    public void UseStamina(float amount)
    {
        if (!Object.HasInputAuthority || currentStamina <= 0f) return;
        if (isRecovering) return;

        float before = currentStamina;
        currentStamina = Mathf.Clamp(currentStamina - amount, 0, maxStamina);

        if (before != currentStamina)
            EventBus<StaminaChanged>.Raise(new StaminaChanged(this, currentStamina, maxStamina));
    }

    public void HealStamina(int amount)
    {
        if (!Object.HasInputAuthority) return;

        currentStamina = Mathf.Clamp(currentStamina + amount, 0, maxStamina);
        EventBus<StaminaChanged>.Raise(new StaminaChanged(this, currentStamina, maxStamina));
    }

    private void Update()
    {
        if (!Object.HasInputAuthority) return;


        if (imRunning && currentStamina > 0&&!isRecovering)
        {
            timer += Time.deltaTime;

            if (timer >= 0.1f)
            {
                timer = 0f;
                UseStamina(1f);
            }
        }
        else
        {
            timer = 0f;
        }



        if (!IsStamania && !isRecovering && currentStamina < maxStamina)
        {
            recoveryTimer += Time.deltaTime;

            if (recoveryTimer >= 1f)
            {
                recoveryTimer = 0f;
                float before = currentStamina;
                currentStamina = Mathf.Clamp(currentStamina + 10, 0, maxStamina);

                if (before != currentStamina)
                {
                    EventBus<StaminaChanged>.Raise(new StaminaChanged(this, currentStamina, maxStamina));
                }

                //  회복이 시작되면 isRecovering 강제로 꺼버림
                if (currentStamina > 1 && isRecovering)
                {
                    isRecovering = false;
                    Debug.Log("회복 재개됨, 쿨타임 종료");
                }
            }
        }

        // 회복 쿨타임 트리거

        if (!isRecovering && currentStamina <= 1 && !IsStamania)
        {
            isRecovering = true;
            skillTimer = 0f;
            Debug.Log("회복 쿨타임 시작");
        }
        // 회복 쿨타임
        if (isRecovering)
        {
            skillTimer += Time.deltaTime;

            if (skillTimer >= staminaCoolTime)
            {
                currentStamina = 2;
                isRecovering = false;
            }
        }


    }
}
