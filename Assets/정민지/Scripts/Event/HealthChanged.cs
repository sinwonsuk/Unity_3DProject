using Fusion;
using UnityEngine;

public class HealthChanged : IEvent
{
    public int currentHp;
    public int maxHp;
    public NetworkBehaviour playerInfo;

    public HealthChanged(NetworkBehaviour player, int hp,int max)
    {
        playerInfo = player;
        currentHp = hp;
        maxHp = max;
    }
}
