using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.Collections.Unicode;

public class AttackState : BaseState<PlayerStateMachine.PlayerState>
{
    PlayerStateMachine playerStateMachine;

    float time;
    bool attackStart = false;
    bool attackContinue = false;
    public AttackState(PlayerStateMachine.PlayerState key, PlayerStateMachine stateMachine ) : base(key)
    {
        this.playerStateMachine = stateMachine;
    }

    public override void EnterState()
    {

         playerStateMachine.Combat.StartAttack();
         
    }
    public override void ExitState()
    {
        if (playerStateMachine.Object.HasStateAuthority)
        {
            playerStateMachine.SetIsAttackFalse();
            playerStateMachine.NetAnim.Animator.SetBool("RunAttack", false);
            playerStateMachine.NetAnim.Animator.SetBool("Attack", false);
        }

        time = 0.0f;
    }

    public override void FixedUpdateState() 
    {
        //if (!playerStateMachine.Object.HasStateAuthority)
        //    return;

        NetworkInputData data = playerStateMachine.inputHandler.GetNetworkInputData();

        float yaw = data.CameraRotateY;
        Quaternion planarRotation = Quaternion.Euler(0, yaw, 0);

        playerStateMachine.playerController.Rotate(planarRotation);

        if(playerStateMachine.isAttack == true)
        {
            time += playerStateMachine.Runner.DeltaTime;

            if (time < 0.2f)
                return;

            if (playerStateMachine.inputHandler.IsAttackPressed())
            {
                playerStateMachine.Combat.TryQueueNextCombo();

                time = 0.0f;
            }

            if(time > 0.5f)
            {
                playerStateMachine.isAttack = false;
            }

        }

        playerStateMachine.action.Invoke();

        //if (playerStateMachine.IsProxy == true || playerStateMachine.Runner.IsForward == false)
        //    return;

        playerStateMachine.AnimHandler.SetAttackCount(playerStateMachine.AttackCount);
        playerStateMachine.AnimHandler.SetAttackBool(true);
    }

    public void AttackMove()
    {
        if (playerStateMachine.Object.HasInputAuthority && !playerStateMachine.Object.HasStateAuthority)
        {
            if (playerStateMachine.isAttack == false)
                return;

            playerStateMachine.playerController.Move(playerStateMachine.transform.forward * playerStateMachine.Runner.DeltaTime * playerStateMachine.AttackSpeed);
        }
        if (playerStateMachine.Object.HasStateAuthority)
        {
            if (playerStateMachine.isAttack == false)
                return;

            playerStateMachine.playerController.Move(playerStateMachine.transform.forward * playerStateMachine.Runner.DeltaTime * playerStateMachine.AttackSpeed);
        }

        
    }


    public override PlayerStateMachine.PlayerState GetNextState()
    {
        return PlayerStateMachine.PlayerState.Attack;
    }

    public override void OnTriggerEnter(Collider collider) { }
    public override void OnTriggerExit(Collider collider) { }
    public override void OnTriggerStay(Collider collider) { }
    public override void OnAttackAnimationEnd()
    {
        playerStateMachine.Combat.OnAnimationEnd();
    }


}