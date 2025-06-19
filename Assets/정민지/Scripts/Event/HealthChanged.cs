using Fusion;
using UnityEngine;

public class HealthChanged : IEvent
{
    public int currentHp;
    public int maxHp;
    public PlayerRef player;

    public HealthChanged(PlayerRef player, int hp,int max)
    {
        this.player = player;
        currentHp = hp;
        maxHp = max;
    }
}
