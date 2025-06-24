using Fusion;
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

        playerStateMachine.AnimHandler.SetAttackBool(true);
        playerStateMachine.cameraManager.isCameraCheck = false;


        if (playerStateMachine.Object.HasInputAuthority)
            zoomRoutine = playerStateMachine.StartCoroutine(playerStateMachine.cameraManager.ZoomDistance(1f));
    }
    public override void ExitState()
    {
        playerStateMachine.AnimHandler.SetAttackBool(false);
        playerStateMachine.cameraManager.isCameraCheck = false;
        isAttack = false;

        if (playerStateMachine.Object.HasInputAuthority)
        {
            if (zoomRoutine != null)
                playerStateMachine.StopCoroutine(zoomRoutine);

            zoomRoutine = playerStateMachine.StartCoroutine(playerStateMachine.cameraManager.ZoomDistance(2f));
        }
    }

    public override void FixedUpdateState()
    {
        if (!playerStateMachine.HasInputAuthority)
            return;

        playerStateMachine.AnimHandler.ChangeMagicAttackState(attack);


        if (playerStateMachine.GetInput(out NetworkInputData data))
        {
            Quaternion quaternion = Quaternion.Euler(0, data.CameraRotateY, 0);
            playerStateMachine.Rotation(data);
        }

        if (playerStateMachine.inputHandler.IsAttackPressed() && playerStateMachine.inputHandler.IsRightAttackPressed())
        {
            isAttack = true;


            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


            if (Physics.Raycast(ray, out var hit, 999f, playerStateMachine.ArrowHitMask))
            {
                targetPos = hit.point;
            }
            else
            {
                float fallbackDist = 50f;
                targetPos = ray.origin + ray.direction * fallbackDist;
            }
            PlayerRef me = playerStateMachine.Object.InputAuthority;
            playerStateMachine.WeaponManager.RequestMagic(playerStateMachine.WeaponManager.magicState, HandSide.Right, me);

            playerStateMachine.SetShootObject(targetPos, playerStateMachine.WeaponManager.magicState);

        }

        else if (playerStateMachine.inputHandler.IsRightAttackPressed() && isAttack == false)
        {
            attack = Mathf.MoveTowards(attack, 0.5f, playerStateMachine.Runner.DeltaTime * speed);
            playerStateMachine.AnimHandler.ChangeMagicAttackState(attack);
        }
        else
        {
            attack = Mathf.MoveTowards(attack, 0.0f, playerStateMachine.Runner.DeltaTime * speed);
            playerStateMachine.AnimHandler.ChangeMagicAttackState(attack);
        }

        if (attack == 0)
        {
            playerStateMachine.RPC_BroadcastState(PlayerStateMachine.PlayerState.Idle);
            return;
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
    NetworkObject magic;
    float attack = 0;
    float speed = 1;
    bool isAttack = false;
    Coroutine zoomRoutine;
}