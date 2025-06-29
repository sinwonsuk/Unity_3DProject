using Fusion;
using UnityEngine;
using static Unity.Collections.Unicode;

public class PortionState : BaseState<PlayerStateMachine.PlayerState>
{
    PlayerStateMachine playerStateMachine;


    public PortionState(PlayerStateMachine.PlayerState key, PlayerStateMachine stateMachine) : base(key)
    {
        this.playerStateMachine = stateMachine;
    }

    public override void EnterState()
    {



        if(playerStateMachine.HasInputAuthority)
        {
            playerStateMachine.slot.quantity -= 1;
            playerStateMachine.AnimHandler.SetPoitionBool(true);
        }
    }
    public override void ExitState()
    {
        playerStateMachine.WeaponManager.potionState = PotionState.none;
        playerStateMachine.AnimHandler.SetPoitionBool(false);

        if (playerStateMachine.slot.quantity == 0 && playerStateMachine.HasStateAuthority)
        {
            playerStateMachine.Runner.Despawn(playerStateMachine.WeaponManager.currentWeapon);
            playerStateMachine.WeaponManager.currentWeapon = null;
        }
    }

    public override void FixedUpdateState()
    {
     
    }

    public override PlayerStateMachine.PlayerState GetNextState()
    {
        return PlayerStateMachine.PlayerState.Portion;
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
