using Fusion;
using UnityEngine;

public struct HealthChanged : IEvent
{
    public int currentHp;
    public int maxHp;
    public PlayerHealth playerInfo;

    public HealthChanged(PlayerHealth player, int hp, int max)
    {
        playerInfo = player;
        currentHp = hp;
        maxHp = max;
    }
}
