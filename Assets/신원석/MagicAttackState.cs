using Fusion;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Collections.Unicode;


public class MagicAttackState : BaseState<PlayerStateMachine.PlayerState>
{

    public MagicAttackState(PlayerStateMachine.PlayerState key, Animator animator, PlayerStateMachine stateMachine) : base(key)
    {
        this.playerStateMachine = stateMachine;
    }

    public override void EnterState()
    {

        playerStateMachine.AnimHandler.SetAttackBool(true);
        playerStateMachine.cameraManager.isCameraCheck = false;

        if (playerStateMachine.Object.HasStateAuthority)
        {
            PlayerRef me = playerStateMachine.Object.InputAuthority;
            playerStateMachine.WeaponManager.RequestMagic(playerStateMachine.WeaponManager.magicState, HandSide.Right, me);
            magic = playerStateMachine.WeaponManager.Magic;
        }



        if (playerStateMachine.Object.HasInputAuthority)
            zoomRoutine = playerStateMachine.StartCoroutine(playerStateMachine.cameraManager.ZoomDistance(1f));
    }
    public override void ExitState()
    {
        playerStateMachine.AnimHandler.SetAttackBool(false);
        playerStateMachine.cameraManager.isCameraCheck = false;
        isAttackTrigger = false;

        if (playerStateMachine.Object.HasInputAuthority)
        {
            if (zoomRoutine != null)
                playerStateMachine.StopCoroutine(zoomRoutine);
            zoomRoutine = playerStateMachine.StartCoroutine(playerStateMachine.cameraManager.ZoomDistance(2f));
        }

    }

    public override void FixedUpdateState()
    {

        if (!playerStateMachine.Object.HasInputAuthority && !playerStateMachine.Runner.IsForward)
            return;

        if (playerStateMachine.GetInput(out NetworkInputData data))
            playerStateMachine.Rotation(data);


        bool left = playerStateMachine.inputHandler.IsAttackPressed();
        bool right = playerStateMachine.inputHandler.IsRightAttackPressed();


        if(left && right && attack >= maxAttack  && playerStateMachine.cameraManager.isCameraCheck == true)
        {
            isAttackTrigger = true;
            CalculateHitPosition();

            playerStateMachine.SetShootObject(targetPos, playerStateMachine.WeaponManager.magicState);
        }
        else if (right && !isAttackTrigger )
            attack = Mathf.MoveTowards(attack, maxAttack, playerStateMachine.Runner.DeltaTime * speed);
        else
            attack = Mathf.MoveTowards(attack, 0f, playerStateMachine.Runner.DeltaTime * speed);


        // (발사 후) Idle 전환
        if (attack <= 0f)
            playerStateMachine.BroadcastIdleEvent(PlayerStateMachine.PlayerState.Idle);

    }


    public override PlayerStateMachine.PlayerState GetNextState()
    {
        return PlayerStateMachine.PlayerState.Magic;
    }
    public void CalculateHitPosition()
    {
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
    }
    public override void OnTriggerEnter(Collider collider) { }
    public override void OnTriggerExit(Collider collider) { }
    public override void OnTriggerStay(Collider collider) { }
    public override void OnHitAnimationEvent()
    {
        playerStateMachine.Combat.OnAttackAnimationEnd();
    }

    Vector3 targetPos;
    float attack = 0;
    float speed = 1;
    bool isAttackTrigger = false;
    float maxAttack = 0.5f;
    Coroutine zoomRoutine;
    PlayerStateMachine playerStateMachine;
    PlayerRef me;
    NetworkObject magic;
}



//using Fusion;
//using System.Collections;
//using UnityEngine;

//public class MagicAttackState : BaseState<PlayerStateMachine.PlayerState>
//{
//    private PlayerStateMachine playerStateMachine;
//    private float attack = 0f;
//    private const float maxAttack = 0.5f;
//    private const float buildSpeed = 1f;
//    private bool isAttackTriggered = false;
//    private Vector3 targetPos;
//    private Coroutine zoomRoutine;

//    public MagicAttackState(PlayerStateMachine.PlayerState key, Animator animator, PlayerStateMachine stateMachine)
//        : base(key)
//    {
//        this.playerStateMachine = stateMachine;
//    }

//    public override void EnterState()
//    {
//        playerStateMachine.AnimHandler.SetAttackBool(true);
//        playerStateMachine.cameraManager.isCameraCheck = false;

//        if (playerStateMachine.Object.HasInputAuthority)
//        {
//            zoomRoutine = playerStateMachine.StartCoroutine(
//                playerStateMachine.cameraManager.ZoomDistance(1f)
//            );
//        }

//        attack = 0f;
//        isAttackTriggered = false;
//    }

//    public override void ExitState()
//    {
//        playerStateMachine.AnimHandler.SetAttackBool(false);
//        playerStateMachine.cameraManager.isCameraCheck = false;
//        isAttackTriggered = false;

//        if (playerStateMachine.Object.HasInputAuthority)
//        {
//            if (zoomRoutine != null)
//                playerStateMachine.StopCoroutine(zoomRoutine);

//            zoomRoutine = playerStateMachine.StartCoroutine(
//                playerStateMachine.cameraManager.ZoomDistance(2f)
//            );
//        }
//    }

//    public override void FixedUpdateState()
//    {
//        // 1) Input 예측: 클라이언트에서 차징 및 애니메이션
//        if (playerStateMachine.Object.HasInputAuthority)
//        {
//            if (playerStateMachine.GetInput(out NetworkInputData data))
//            {
//                playerStateMachine.Rotation(data);
//            }

//            bool left = playerStateMachine.inputHandler.IsAttackPressed();
//            bool right = playerStateMachine.inputHandler.IsRightAttackPressed();


//            // 발사 요청
//            if (left && right && attack >= maxAttack && !isAttackTriggered)
//            {
//                isAttackTriggered = true;
//                CalculateTargetPosition();
//                playerStateMachine.SetShootObject(
//                    targetPos,
//                    playerStateMachine.WeaponManager.magicState
//                );
//            }

//            // 차징 처리
//            else if (right && !isAttackTriggered)
//            {
//                attack = Mathf.MoveTowards(
//                    attack, maxAttack,
//                    playerStateMachine.Runner.DeltaTime * buildSpeed
//                );
//            }
//            else
//            {
//                attack = Mathf.MoveTowards(
//                    attack, 0f,
//                    playerStateMachine.Runner.DeltaTime * buildSpeed
//                );
//            }
//            playerStateMachine.AnimHandler.ChangeMagicAttackState(attack);

            
//            return;
//        }

//        // 2) 서버 권위: Idle 전환만
//        if (playerStateMachine.Object.HasStateAuthority)
//        {
//            if (attack <= 0f)
//            {
//                playerStateMachine.BroadcastIdleEvent(
//                    PlayerStateMachine.PlayerState.Idle
//                );
//            }
//        }
//    }

//    private void CalculateTargetPosition()
//    {
//        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//        if (Physics.Raycast(ray, out RaycastHit hit, 999f, playerStateMachine.ArrowHitMask))
//        {
//            targetPos = hit.point;
//        }
//        else
//        {
//            targetPos = ray.origin + ray.direction * 50f;
//        }
//    }

//    public override PlayerStateMachine.PlayerState GetNextState()
//    {
//        return PlayerStateMachine.PlayerState.Magic;
//    }

//    public override void OnTriggerEnter(Collider collider) { }
//    public override void OnTriggerExit(Collider collider) { }
//    public override void OnTriggerStay(Collider collider) { }

//    public override void OnHitAnimationEvent()
//    {
//        //playerStateMachine.Combat.OnAttackAnimationEnd();
//    }
//}
