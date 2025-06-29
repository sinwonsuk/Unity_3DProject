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
    float stamina;


    public AttackState(PlayerStateMachine.PlayerState key, PlayerStateMachine stateMachine ) : base(key)
    {
        this.playerStateMachine = stateMachine;
       

    }

    public override void EnterState()
    {
        playerStateMachine.Combat.StartAttack();
        playerStateMachine.hitSet.Clear();
        if (!playerStateMachine.HasInputAuthority)
            return;


        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Sword,false);
    }
    public override void ExitState()
    {
        if (playerStateMachine.Object.HasStateAuthority)
        {
            playerStateMachine.SetIsAttackFalse();
            playerStateMachine.NetAnim.Animator.SetBool("RunAttack", false);
            playerStateMachine.NetAnim.Animator.SetBool("Attack", false);
        }
        //playerStateMachine.Stamina.IsStamania = false;
        playerStateMachine.Combat.hasConsumedStamina = false;
        time = 0.0f;
    }

    public override void FixedUpdateState() 
    {
        if (playerStateMachine.AttackCount >= 5)
            playerStateMachine.BroadcastIdleEvent(PlayerState.Idle);


        playerStateMachine.action.Invoke();

        if(playerStateMachine.SoundCheck ==true)
        {
            //SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Sword, false);
            playerStateMachine.SoundCheck = false;
        }



        if (!playerStateMachine.Object.HasStateAuthority && !playerStateMachine.Runner.IsForward)
            return;

        playerStateMachine.AnimHandler.SetAttackCount(playerStateMachine.AttackCount);


        NetworkInputData data = playerStateMachine.inputHandler.GetNetworkInputData();

        float yaw = data.CameraRotateY;
        Quaternion planarRotation = Quaternion.Euler(0, yaw, 0);

        playerStateMachine.Rotation(data);

        if(playerStateMachine.isAttack == true && playerStateMachine.Combat.hasConsumedStamina == false)
        {
            time += playerStateMachine.Runner.DeltaTime;

            if (time < 0.1f)
                return;

            if (playerStateMachine.inputHandler.IsAttackPressed() && playerStateMachine.Stamina.currentStamina >= 0.0f)
            {
                //playerStateMachine.Stamina.ConsumeStaminaOnServer(playerStateMachine.Combat.stamina);

                playerStateMachine.Combat.TryQueueNextCombo();
                time = 0.0f;
                playerStateMachine.Combat.hasConsumedStamina = true;
            }

        }
       
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
        // 1) 호스트에서만 충돌 처리
        if (!playerStateMachine.Object.HasStateAuthority)
            return;


        if (playerStateMachine.isDeath == true)
            return;

        // 2) Weapon 네트워크 오브젝트 가져오기
        var weaponNetObj = collider.GetComponent<NetworkObject>();
        if (weaponNetObj == null || !collider.CompareTag("Weapon"))
            return;


        // 3) Weapon의 입력 권한자가 이 플레이어와 같다면 스킵
        if (weaponNetObj.InputAuthority == playerStateMachine.Object.InputAuthority)
            return;

        int attack = weaponNetObj.gameObject.GetComponent<WeaponNetworkObject>().weaponInfoConfig.Attack;

        playerStateMachine.health.RequestDamage(attack);

        weaponNetObj.GetComponent<WeaponNetworkObject>().GetComponent<MeshCollider>().enabled = false;

        playerStateMachine.BroadcastIdleEvent(PlayerState.Hit);
    }
    public override void OnTriggerExit(Collider collider) { }
    public override void OnTriggerStay(Collider collider) { }
    public override void OnAnimationEvent()
    {
        playerStateMachine.Combat.OnAttackAnimationEnd();
    }


}