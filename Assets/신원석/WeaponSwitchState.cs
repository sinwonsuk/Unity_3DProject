using Fusion;
using UnityEngine;
using UnityEngine.AI;

public class WeaponSwitchState : BaseState<PlayerStateMachine.PlayerState>
{
    PlayerStateMachine playerStateMachine;
    public WeaponSwitchState(PlayerStateMachine.PlayerState key,PlayerStateMachine playerStateMachine) : base(key)
    {
        this.playerStateMachine = playerStateMachine;
    }

    public override void EnterState()
    {
        if (playerStateMachine.Object.HasStateAuthority)
        {
            PlayerRef me = playerStateMachine.Object.InputAuthority;
            playerStateMachine.SetWeapon(true);
            playerStateMachine.WeaponManager.RequestEquip(ItemState.Sword, HandSide.Right, me);
            playerStateMachine.BroadcastIdleEvent(PlayerStateMachine.PlayerState.Idle);
        }

        playerStateMachine.AnimHandler.ChangeWeapon(ItemState.Sword);
    }
    public override void ExitState()
    {




    }

    public override PlayerStateMachine.PlayerState GetNextState()
    {      
        return PlayerStateMachine.PlayerState.Switch;
    }

    public override void OnTriggerEnter(Collider collider) { }
    public override void OnTriggerExit(Collider collider) { }
    public override void OnTriggerStay(Collider collider) { }

    public override void FixedUpdateState(){ }

    public override void OnHitAnimationEvent()
    {

    }
}
