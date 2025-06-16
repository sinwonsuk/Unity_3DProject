using UnityEngine;

public class Heal : IEvent
{
    public int heal;

    public Heal(int heal)
    {
        this.heal = heal;
    }
}
