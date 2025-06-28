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
        // 1) �Է� ����, ī�޶� Ȱ�� ���� üũ
        if (!playerStateMachine.HasInputAuthority ||
            !playerStateMachine.cameraManager.isCameraCheck)
        {
            return;
        }

        // 2) ���¹̳� �� ���� ���� ���� üũ
        float currentStamina = playerStateMachine.Stamina.currentStamina;
        float cost = playerStateMachine.AttackStaminaCost;
        float jumpCcost = playerStateMachine.JumpStaminaCost;
        int weaponCount = playerStateMachine.AnimHandler.WeaponCount;
        bool hasWeapon = playerStateMachine.IsWeapon;
        PotionState poition  = playerStateMachine.WeaponManager.potionState;

        var input = playerStateMachine.inputHandler;

        // 3) ���� �Է� ó��
        if (input.IsAttackPressed() &&
            weaponCount != (int)ItemState.Bow &&
            weaponCount != (int)ItemState.FireMagic &&
            currentStamina > cost &&
            hasWeapon == true)
        {
            playerStateMachine.BroadcastIdleEvent(PlayerState.Attack);
            return;
        }

        // 4) Ȱ������ ���� ����
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
        // 5) ���� �Է�
        if (input.IsCtrlButtonPress() && currentStamina > jumpCcost)
        {
            playerStateMachine.BroadcastIdleEvent(PlayerState.Jump);
            return;
        }

     

        // 6) �̵� �Է�
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
        // 1) ȣ��Ʈ������ �浹 ó��
        if (!playerStateMachine.Object.HasStateAuthority)
            return;

        // 2) Weapon ��Ʈ��ũ ������Ʈ ��������
        var weaponNetObj = collider.GetComponent<NetworkObject>();
        if (weaponNetObj == null || !collider.CompareTag("Weapon"))
            return;

        if(collider.CompareTag("Weapon") && collider.CompareTag("Arrow") && collider.CompareTag("Magic"))
        {
            int a = 0;
        }

        // 3) Weapon�� �Է� �����ڰ� �� �÷��̾�� ���ٸ� ��ŵ
        if (weaponNetObj.InputAuthority == playerStateMachine.Object.InputAuthority)
            return;




        // 4) ��¥ Ÿ�� ó��
        Debug.Log("�浹 ����!2");

        playerStateMachine.BroadcastIdleEvent(PlayerState.Hit);

    }

    public override void OnTriggerExit(Collider collider) { }
    public override void OnTriggerStay(Collider collider) { }
    public override void OnAnimationEvent()
    {

    }
}