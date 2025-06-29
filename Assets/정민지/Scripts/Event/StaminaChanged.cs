using Fusion;
using UnityEngine;

public class StaminaChanged : IEvent
{
    public float currentStamina;
    public int maxStamina;
    public NetworkBehaviour _playerInfo;

    public StaminaChanged(NetworkBehaviour player,float stamina,int maxStamina)
    {
        _playerInfo = player;
        currentStamina = stamina;
        this.maxStamina = maxStamina;
    }
}
