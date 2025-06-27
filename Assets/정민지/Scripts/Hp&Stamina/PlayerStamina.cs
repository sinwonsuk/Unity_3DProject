using Fusion;
using UnityEngine;

public class PlayerStamina : NetworkBehaviour
{
    private int currentStamina;
    [SerializeField] private int maxStamina = 100;
    [SerializeField] private float recoveryTimer;

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            currentStamina = maxStamina;
            EventBus<StaminaChanged>.Raise(new StaminaChanged(this, currentStamina, maxStamina));
        }
    }

    public void UseStamina(int amount)
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

        if (currentStamina < maxStamina)
        {
            recoveryTimer += Time.deltaTime;

            if (recoveryTimer >= 1f)
            {
                currentStamina++;
                EventBus<StaminaChanged>.Raise(new StaminaChanged(this, currentStamina, maxStamina));
                recoveryTimer = 0f;
            }
        }
    }
}
