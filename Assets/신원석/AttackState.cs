using Fusion;
using RPGCharacterAnims;
using System.Collections;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using static PlayerStateMachine;
using static Unity.Collections.Unicode;


public class AttackState : BaseState<PlayerStateMachine.PlayerState>
{
    PlayerStateMachine playerStateMachine;

    float time;

    public AttackState(PlayerStateMachine.PlayerState key, PlayerStateMachine stateMachine ) : base(key)
    {
        this.playerStateMachine = stateMachine;
    }

    public override void EnterState()
    {
        playerStateMachine.Combat.StartAttack();
        playerStateMachine.hitSet.Clear();
        playerStateMachine.Stamina.UseStamina(playerStateMachine.Stamina.AttackStaminaCost);
        playerStateMachine.Stamina.IsStamania = true;

    }
    public override void ExitState()
    {
        if (playerStateMachine.Object.HasStateAuthority)
        {
            playerStateMachine.SetIsAttackFalse();
            playerStateMachine.NetAnim.Animator.SetBool("RunAttack", false);
            playerStateMachine.NetAnim.Animator.SetBool("Attack", false);
        }
        playerStateMachine.Stamina.IsStamania = false;
        time = 0.0f;
    }

    public override void FixedUpdateState() 
    {

        if (!playerStateMachine.Object.HasStateAuthority && !playerStateMachine.Runner.IsForward)
            return;

        playerStateMachine.AnimHandler.SetAttackCount(playerStateMachine.AttackCount);


        NetworkInputData data = playerStateMachine.inputHandler.GetNetworkInputData();

        float yaw = data.CameraRotateY;
        Quaternion planarRotation = Quaternion.Euler(0, yaw, 0);

        playerStateMachine.Rotation(data);

        if(playerStateMachine.isAttack == true)
        {
            time += playerStateMachine.Runner.DeltaTime;

            if (time < 0.1f)
                return;

            if (playerStateMachine.inputHandler.IsAttackPressed() && playerStateMachine.Stamina.currentStamina >= 0.0f)
            {
                
                playerStateMachine.Combat.TryQueueNextCombo();
                time = 0.0f;
            }

        }
        playerStateMachine.action.Invoke();      
        AttackMove();
    }




    public void AttackMove()
    {
        if (playerStateMachine.Object.HasStateAuthority)
        {
            if (playerStateMachine.isAttack == false)
                return;

            NetworkInputData data = playerStateMachine.inputHandler.GetNetworkInputData();

            playerStateMachine.playerController.Move(playerStateMachine.transform.forward * playerStateMachine.AttackSpeed);
        }      
    }


    public override PlayerStateMachine.PlayerState GetNextState()
    {
        return PlayerStateMachine.PlayerState.Attack;
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

        // 3) Weapon�� �Է� �����ڰ� �� �÷��̾�� ���ٸ�(self-hit) ��ŵ
        if (weaponNetObj.InputAuthority == playerStateMachine.Object.InputAuthority)
            return;

        // 4) ��¥ Ÿ�� ó��
        Debug.Log("�浹 ����!");

    }
    public override void OnTriggerExit(Collider collider) { }
    public override void OnTriggerStay(Collider collider) { }
    public override void OnHitAnimationEvent()
    {
        playerStateMachine.Combat.OnAttackAnimationEnd();
    }


}