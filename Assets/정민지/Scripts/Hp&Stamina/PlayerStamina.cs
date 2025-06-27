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
    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            currentStamina = maxStamina;
            EventBus<StaminaChanged>.Raise(new StaminaChanged(this, currentStamina, maxStamina));
        }
    }

    public void UseStamina(float amount)
    {
        if (!Object.HasInputAuthority) return;

        currentStamina = Mathf.Clamp(currentStamina - amount, 0, maxStamina);
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

        if (currentStamina < maxStamina && IsStamania ==false)
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
