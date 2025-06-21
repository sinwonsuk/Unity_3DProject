using UnityEngine;
using static PlayerStateMachine;
using static Unity.Collections.Unicode;

public class IdleState : BaseState<PlayerStateMachine.PlayerState>
{
    PlayerStateMachine playerStateMachine;
    public IdleState(PlayerStateMachine.PlayerState key , PlayerStateMachine PlayerStateMachine) : base(key)
    {
        this.playerStateMachine = PlayerStateMachine;
    }

    public override void EnterState()
    {
        playerStateMachine.NetAnim.Animator.SetBool("Attack", false);
    }
    public override void ExitState() => Debug.Log("Exit Idle");
  

    public override void FixedUpdateState()
    {
        if (playerStateMachine.cameraManager.isCameraCheck == false)
            return;

        if (playerStateMachine.inputHandler.IsAttackPressed() && playerStateMachine.Object.HasInputAuthority &&
            playerStateMachine.IsWeapon == true && playerStateMachine.AnimHandler.WeaponCount != (int)ItemState.Bow)
        {
            playerStateMachine.RPC_BroadcastState(PlayerState.Attack);
            return;
        }
        else if (playerStateMachine.inputHandler.IsRightAttackPressed() && playerStateMachine.Object.HasInputAuthority &&
            playerStateMachine.IsWeapon == true && playerStateMachine.AnimHandler.WeaponCount == (int)ItemState.Bow)
        {
            playerStateMachine.RPC_BroadcastState(PlayerState.BowAttack);
            return;
        }

        else if (playerStateMachine.inputHandler.IsRightAttackPressed() && playerStateMachine.Object.HasInputAuthority &&
            playerStateMachine.IsWeapon == true && playerStateMachine.AnimHandler.WeaponCount == (int)ItemState.Magic)
        {
            playerStateMachine.RPC_BroadcastState(PlayerState.Magic);
            return;
        }

        else if (playerStateMachine.inputHandler.IsLButtonPress() && playerStateMachine.Object.HasInputAuthority && playerStateMachine.IsWeapon ==false)
        {
            playerStateMachine.RPC_BroadcastState(PlayerState.Switch);
            return;
        }
       
        else if (playerStateMachine.inputHandler.IsCtrlButtonPress() && playerStateMachine.Object.HasInputAuthority)
        {
            playerStateMachine.RPC_BroadcastState(PlayerState.Jump);
            return;
        }
        else if (playerStateMachine.inputHandler.IsMove() && playerStateMachine.Object.HasInputAuthority)
        {
            playerStateMachine.RPC_BroadcastState(PlayerState.Move);
            return;
        }



    }

    public override PlayerStateMachine.PlayerState GetNextState()
    {
        return PlayerStateMachine.PlayerState.Idle;
    }

   

    public override void OnTriggerEnter(Collider collider) { }
    public override void OnTriggerExit(Collider collider) { }
    public override void OnTriggerStay(Collider collider) { }
    public override void OnAttackAnimationEnd()
    {

    }
}