using UnityEngine;
using Fusion;

public class DeathState : BaseState<PlayerStateMachine.PlayerState>
{
    PlayerStateMachine playerStateMachine;


    public DeathState(PlayerStateMachine.PlayerState key, PlayerStateMachine stateMachine) : base(key)
    {
        this.playerStateMachine = stateMachine;
    }

    public override void EnterState()
    {
        //if (!playerStateMachine.Object.HasStateAuthority)
         //   return;

        playerStateMachine.AnimHandler.ChangeDeathState();
        playerStateMachine.playerController.Collider.enabled = false;
    }
    public override void ExitState()
    {


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
