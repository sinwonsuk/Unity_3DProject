using UnityEngine;
using Fusion;

public class PortionState : BaseState<PlayerStateMachine.PlayerState>
{
    PlayerStateMachine playerStateMachine;


    public PortionState(PlayerStateMachine.PlayerState key, PlayerStateMachine stateMachine) : base(key)
    {
        this.playerStateMachine = stateMachine;
    }

    public override void EnterState()
    {
        if (!playerStateMachine.Object.HasStateAuthority)
            return;


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
