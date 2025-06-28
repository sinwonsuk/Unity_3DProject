using UnityEngine;
using Fusion;

public class PortionState : BaseState<PlayerStateMachine.PlayerState>
{
    PlayerStateMachine playerStateMachine;


    public PortionState(PlayerStateMachine.PlayerState key, PlayerStateMachine stateMachine) : base(key)
    {

       

    }

    public override void EnterState()
    {

        if (!playerStateMachine.Object.HasStateAuthority)
            return;

        if (playerStateMachine.WeaponManager.potionState == PotionState.HpPotion)
        {
            playerStateMachine.health.Heal(30);
        }
        if (playerStateMachine.WeaponManager.potionState == PotionState.StaminaPotion)
        {
            playerStateMachine.Stamina.HealStamina(50);
        }

        playerStateMachine.AnimHandler.SetPoitionBool(true);
    }
    public override void ExitState()
    {
        playerStateMachine.WeaponManager.potionState = PotionState.none;
        playerStateMachine.AnimHandler.SetPoitionBool(false);
    }

    public override void FixedUpdateState()
    {

    }

    public override PlayerStateMachine.PlayerState GetNextState()
    {
        return PlayerStateMachine.PlayerState.Death;
    }

    public override void OnTriggerEnter(Collider collider)
    {


    }
    public override void OnTriggerExit(Collider collider) { }
    public override void OnTriggerStay(Collider collider) { }
    public override void OnAnimationEvent()
    {

    }


}
