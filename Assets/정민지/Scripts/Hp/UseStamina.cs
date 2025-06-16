using UnityEngine;

public class UseStamina : IEvent
{
    public int useStamina;

    public UseStamina(int useStamina)
    {
        this.useStamina = useStamina;
    }
}
