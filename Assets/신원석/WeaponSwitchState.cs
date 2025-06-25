using Fusion;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static PlayerStateMachine;

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

            playerStateMachine.SetWeapon(true);
            playerStateMachine.WeaponManager.RequestEquip(playerStateMachine.itemState, HandSide.Right, playerStateMachine.me1);
            playerStateMachine.AnimHandler.ChangeWeapon(playerStateMachine.itemState);
            //playerStateMachine.BroadcastIdleEvent(PlayerState.Idle);







            //PlayerRef me = playerStateMachine.Object.InputAuthority;
            //playerStateMachine.SetWeapon(true);
            //playerStateMachine.WeaponManager.RequestEquip(ItemState.Harberd, HandSide.Right, me);
            //playerStateMachine.BroadcastIdleEvent(PlayerStateMachine.PlayerState.Idle);
        }

        playerStateMachine.AnimHandler.ChangeWeapon(playerStateMachine.itemState);
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
