using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Damage : IEvent
{
    public int damage;

    public Damage(int damage)
    {
        this.damage = damage;
    }
}
