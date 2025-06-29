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
        // 1) 입력 권한, 카메라 활성 여부 체크
        if (!playerStateMachine.HasInputAuthority ||
            !playerStateMachine.cameraManager.isCameraCheck)
        {
            return;
        }

        // 2) 스태미너 및 무기 보유 여부 체크
        float currentStamina = playerStateMachine.Stamina.currentStamina;
        float cost = playerStateMachine.AttackStaminaCost;
        float jumpCcost = playerStateMachine.JumpStaminaCost;
        int weaponCount = playerStateMachine.AnimHandler.WeaponCount;
        bool hasWeapon = playerStateMachine.IsWeapon;
        PotionState poition  = playerStateMachine.WeaponManager.potionState;

        var input = playerStateMachine.inputHandler;

        // 3) 공격 입력 처리
        if (input.IsAttackPressed() &&
            weaponCount != (int)ItemState.Bow &&
            weaponCount != (int)ItemState.FireMagic &&
            currentStamina > cost &&
            hasWeapon == true)
        {
            playerStateMachine.BroadcastIdleEvent(PlayerState.Attack);
            return;
        }

        // 4) 활·마법 공격 구분
        if (input.IsRightAttackPressed() && currentStamina > cost && hasWeapon ==true)
        {
            if (weaponCount == (int)ItemState.Bow )
                playerStateMachine.BroadcastIdleEvent(PlayerState.BowAttack);
            else if (weaponCount == (int)ItemState.FireMagic)
                playerStateMachine.BroadcastIdleEvent(PlayerState.Magic);

            return;
        }

        if (input.IsAttackPressed() && (poition == PotionState.HpPotion || poition == PotionState.StaminaPotion))
        {
            playerStateMachine.BroadcastIdleEvent(PlayerState.Portion);
            return;
        }
        // 5) 점프 입력
        if (input.IsCtrlButtonPress() && currentStamina > jumpCcost)
        {
            playerStateMachine.BroadcastIdleEvent(PlayerState.Jump);
            return;
        }

     

        // 6) 이동 입력
        if (input.IsMove())
        {
            playerStateMachine.BroadcastIdleEvent(PlayerState.Move);
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

        if(collider.CompareTag("Weapon") && collider.CompareTag("Arrow") && collider.CompareTag("Magic"))
        {
            int a = 0;
        }

        // 3) Weapon의 입력 권한자가 이 플레이어와 같다면 스킵
        if (weaponNetObj.InputAuthority == playerStateMachine.Object.InputAuthority)
            return;




        // 4) 진짜 타격 처리
        Debug.Log("충돌 감지!2");

        playerStateMachine.BroadcastIdleEvent(PlayerState.Hit);

    }

    public override void OnTriggerExit(Collider collider) { }
    public override void OnTriggerStay(Collider collider) { }
    public override void OnAnimationEvent()
    {

    }
}