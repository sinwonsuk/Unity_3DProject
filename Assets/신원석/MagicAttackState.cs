using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;


public class MagicAttackState : BaseState<PlayerStateMachine.PlayerState>
{
    PlayerStateMachine playerStateMachine;

    public MagicAttackState(PlayerStateMachine.PlayerState key, Animator animator, PlayerStateMachine stateMachine) : base(key)
    {
        this.playerStateMachine = stateMachine;
    }

    public override void EnterState()
    {
        playerStateMachine.StartCoroutine(playerStateMachine.cameraManager.StartCameraScaleUp());
        playerStateMachine.AnimHandler.SetAttackBool(true);
    }
    public override void ExitState()
    {
        playerStateMachine.StartCoroutine(playerStateMachine.cameraManager.StartCameraScaleDown());
        playerStateMachine.AnimHandler.SetAttackBool(false);
        isAttack = false;
    }

    public override void FixedUpdateState()
    {
        if (!playerStateMachine.HasInputAuthority)
            return;

        if(attack == 0 && isAttack ==true)
        {
            playerStateMachine.ChangeState(PlayerStateMachine.PlayerState.Idle);
            return;
        }
            
        if (playerStateMachine.GetInput(out NetworkInputData data))
        {
            Quaternion quaternion = Quaternion.Euler(0, data.CameraRotateY, 0);

           // playerStateMachine.playerController.Rotate(quaternion);
        }


        if (playerStateMachine.inputHandler.IsAttackPressed() && playerStateMachine.inputHandler.IsRightAttackPressed())
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


            if (Physics.Raycast(ray, out var hit, 999f, playerStateMachine.LayerMask))
            {
                targetPos = hit.point;
            }
            else
            {
                float fallbackDist = 50f;
                targetPos = ray.origin + ray.direction * fallbackDist;
            }
            adad = playerStateMachine.WeaponManager.CreateMagic();
            //adad.GetComponent<Arrow>().Shoot(targetPos);

            //playerStateMachine.AnimHandler.ChangeMagicAttackState(attack);
        }

        else if (playerStateMachine.inputHandler.IsRightAttackPressed())
        {
            isAttack = true;
            attack = Mathf.MoveTowards(attack, 0.5f, playerStateMachine.Runner.DeltaTime * speed);
            playerStateMachine.AnimHandler.ChangeMagicAttackState(attack);
        }
        else
        {
            attack = Mathf.MoveTowards(attack, 0.0f, playerStateMachine.Runner.DeltaTime * speed);
            playerStateMachine.AnimHandler.ChangeMagicAttackState(attack);
        }
    }

    public override PlayerStateMachine.PlayerState GetNextState()
    {
        return PlayerStateMachine.PlayerState.Magic;
    }
    public void ChangeRotate()
    {

    }
    public override void OnTriggerEnter(Collider collider) { }
    public override void OnTriggerExit(Collider collider) { }
    public override void OnTriggerStay(Collider collider) { }
    public override void OnHitAnimationEvent()
    {
        playerStateMachine.Combat.OnAttackAnimationEnd();
    }

    Vector3 targetPos;
    GameObject adad;
    float attack = 0;
    float speed = 1;
    bool isAttack = false;
}