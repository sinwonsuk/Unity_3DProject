using Fusion;
using UnityEngine;

public class StaminaChanged : IEvent
{
    public int currentStamina;
    public int maxStamina;
    public NetworkBehaviour _playerInfo;

    public StaminaChanged(NetworkBehaviour player,int stamina,int maxStamina)
    {
        _playerInfo = player;
        currentStamina = stamina;
        this.maxStamina = maxStamina;
    }
}
