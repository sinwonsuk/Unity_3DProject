using UnityEngine;

public class Stamina : IEvent
{
    public int currentStamina;

    public Stamina(int stamina)
    {
        currentStamina = stamina;
    }
}
