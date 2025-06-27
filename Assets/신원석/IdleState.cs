using Fusion;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime;
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
        playerStateMachine.NetAnim.Animator.SetBool("Hit", false);
    }
    public override void ExitState() => Debug.Log("Exit Idle");
  

    public override void FixedUpdateState()
    {
        if (playerStateMachine.HasInputAuthority == false)
            return;


        if (playerStateMachine.cameraManager.isCameraCheck == false)
            return;

        if (playerStateMachine.inputHandler.IsAttackPressed() && playerStateMachine.Object.HasInputAuthority &&
            playerStateMachine.IsWeapon == true && playerStateMachine.AnimHandler.WeaponCount != (int)ItemState.Bow && playerStateMachine.AnimHandler.WeaponCount != 4)
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
            playerStateMachine.IsWeapon == true && playerStateMachine.AnimHandler.WeaponCount ==4)
        {
            playerStateMachine.RPC_BroadcastState(PlayerState.Magic);
            return;
        }

        //else if (playerStateMachine.inputHandler.IsLButtonPress() && playerStateMachine.Object.HasInputAuthority && playerStateMachine.IsWeapon ==false)
        //{
        //    playerStateMachine.RPC_BroadcastState(PlayerState.Switch);
        //    return;
        //}

        else if (Input.GetKey(KeyCode.K) && playerStateMachine.Object.HasInputAuthority && playerStateMachine.IsWeapon == false)
        {
            playerStateMachine.RPC_BroadcastState(PlayerState.Switch);
            return;
        }


        else if (playerStateMachine.inputHandler.IsCtrlButtonPress() && playerStateMachine.Object.HasInputAuthority)
        {
           // playerStateMachine.playerController.Move(Vector3.zero,5);
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

   

    public override void OnTriggerEnter(Collider collider)
    {


        // 1) 호스트에서만 충돌 처리
        if (!playerStateMachine.Object.HasStateAuthority)
            return;

        // 2) Weapon 네트워크 오브젝트 가져오기
        var weaponNetObj = collider.GetComponent<NetworkObject>();
        if (weaponNetObj == null || !collider.CompareTag("Weapon"))
            return;

        // 3) Weapon의 입력 권한자가 이 플레이어와 같다면 스킵
        if (weaponNetObj.InputAuthority == playerStateMachine.Object.InputAuthority)
            return;

        // 4) 진짜 타격 처리
        Debug.Log("충돌 감지!2");

        playerStateMachine.health.TakeDamage(10);
        playerStateMachine.RPC_PlayHit();

        //playerStateMachine.BroadcastIdleEvent(PlayerState.Hit);


    }

    public override void OnTriggerExit(Collider collider) { }
    public override void OnTriggerStay(Collider collider) { }
    public override void OnHitAnimationEvent()
    {

    }
}