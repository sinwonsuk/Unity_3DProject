using Fusion;
using UnityEngine;

public class StaminaChanged : IEvent
{
    public float currentStamina;
    public int maxStamina;


    public StaminaChanged(float stamina,int maxStamina)
    {
        currentStamina = stamina;
        this.maxStamina = maxStamina;
    }
}
