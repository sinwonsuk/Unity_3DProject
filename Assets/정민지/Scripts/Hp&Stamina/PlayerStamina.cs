using Fusion;
using UnityEngine;

public class PlayerStamina : NetworkBehaviour
{
    public float currentStamina { get; set; }

    public float AttackStaminaCost { get; set; } = 10f;

    public float MagicStaminaCost { get; set; } = 10f;

    public float ArrowStaminaCost { get; set; } = 10f;

    [SerializeField] private int maxStamina = 100;
    [SerializeField] private float recoveryTimer;
    public bool IsStamania { get; set; } = false;
    private bool imRunning = false;
    private float timer = 0f;

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


        if (imRunning && currentStamina > 0)
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


        if (IsStamania == true)      
            return;

        if (currentStamina < maxStamina)
        {
            recoveryTimer += Time.deltaTime;

            if (recoveryTimer >= 1f)
            {
                float before = currentStamina;
                currentStamina = Mathf.Clamp(currentStamina + 1, 0, maxStamina);

                if (before != currentStamina)
                {
                    EventBus<StaminaChanged>.Raise(new StaminaChanged(this, currentStamina, maxStamina));
                }

                recoveryTimer = 0f;
            }
        }

    }
}
