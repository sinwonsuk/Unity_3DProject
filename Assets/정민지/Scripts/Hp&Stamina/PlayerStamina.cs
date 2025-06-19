using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : NetworkBehaviour
{
    [Networked] public int currentStamina { get; private set; }
    [SerializeField] private int maxStamina;
    [SerializeField] private float recoveryTimer;

    public override void Spawned()
    {
        if (HasStateAuthority)
            currentStamina = maxStamina;

        if (Object.HasInputAuthority)
        {
            EventBus<StaminaChanged>.Raise(new StaminaChanged(this, currentStamina, maxStamina));
        }
    }

    public void UseStamina(int stamina)
    {
        if (!HasStateAuthority) return;

        currentStamina = Mathf.Clamp(currentStamina - stamina, 0, maxStamina);

        if (Object.HasInputAuthority)
        {
            EventBus<StaminaChanged>.Raise(new StaminaChanged(this, currentStamina, maxStamina));
        }
    }

    public void UseStaminaHealingItem(int stamina)
    {
        if (!HasStateAuthority) return;

        currentStamina = Mathf.Clamp(currentStamina + stamina, 0, maxStamina);

        if (Object.HasInputAuthority)
        {
            EventBus<StaminaChanged>.Raise(new StaminaChanged(this, currentStamina, maxStamina));
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;

        if (currentStamina < maxStamina)
        {
            recoveryTimer += Time.deltaTime;

            if (recoveryTimer >= 1f)
            {
                currentStamina++;
                if (Object.HasInputAuthority)
                {
                    EventBus<StaminaChanged>.Raise(new StaminaChanged(this, currentStamina, maxStamina));
                }
                recoveryTimer = 0f;
            }
        }
    }
}