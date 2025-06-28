using Fusion;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;




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
            //PlayerRef me = playerStateMachine.Object.InputAuthority;
            //playerStateMachine.WeaponManager.RequestMagic(playerStateMachine.WeaponManager.magicState, HandSide.Right, me);
            //magic = playerStateMachine.WeaponManager.Magic;
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

        if (fireRoutine != null)
        {
            playerStateMachine.StopCoroutine(fireRoutine);
            fireRoutine = null;
        }
        attackCooldown = 0.0f;
    }

    public override void FixedUpdateState()
    {
        if (playerStateMachine.fireTimer.Expired(playerStateMachine.Runner))
        {
            playerStateMachine.SetShootMagicObject(targetPos, playerStateMachine.WeaponManager.magicState);
            // 발사 이펙트 등 로컬 연출(원하면)

            playerStateMachine.fireTimer = TickTimer.None;
        }

        if (!playerStateMachine.Runner.IsForward)
            return;

        if (playerStateMachine.GetInput(out NetworkInputData data))
            playerStateMachine.Rotation(data);


        bool left = playerStateMachine.inputHandler.IsAttackPressed();
        bool right = playerStateMachine.inputHandler.IsRightAttackPressed();



        if (left && right && attack >= maxAttack && playerStateMachine.cameraManager.isCameraCheck == true && isAttackTrigger ==false)
        {
            // 발사 위치 계산
            CalculateHitPosition();

            // RPC로 서버 발사 요청
            playerStateMachine.RPC_RequestMagic(playerStateMachine.WeaponManager.magicState, HandSide.Right);

            // 쿨타임 시작
            playerStateMachine.fireTimer = TickTimer.CreateFromSeconds(playerStateMachine.Runner, fireDelay);


            attackCooldown = 0.2f;

            isAttackTrigger = true;





        }
        if (right)
            attack = Mathf.MoveTowards(attack, maxAttack, playerStateMachine.Runner.DeltaTime * speed);
        else
            attack = Mathf.MoveTowards(attack, 0f, playerStateMachine.Runner.DeltaTime * speed);


        if (isAttackTrigger == true)
            attackCooldown -= playerStateMachine.Runner.DeltaTime;

        if(isAttackTrigger == true && attackCooldown <=0)
        {
            playerStateMachine.SetShootMagicObject(targetPos, playerStateMachine.WeaponManager.magicState);
            attackCooldown = 0.2f;
            isAttackTrigger = false;
        }




        // (발사 후) Idle 전환
        if (attack <= 0f)
            playerStateMachine.BroadcastIdleEvent(PlayerStateMachine.PlayerState.Idle);

    }

    // 딜레이 후 발사 실행
    private IEnumerator DelayedFire()
    {
        playerStateMachine.RPC_RequestMagic(playerStateMachine.WeaponManager.magicState, HandSide.Right);
        yield return new WaitForSeconds(fireDelay);
        fireRoutine = null;  // 코루틴 완료 표시

        // 서버에 RPC 요청하여 발사 처리
        if (playerStateMachine.Object.HasInputAuthority)
        {           
            playerStateMachine.SetShootMagicObject(targetPos, playerStateMachine.WeaponManager.magicState);
        }
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
            Debug.DrawLine(ray.origin, hit.point, Color.red, 10f);
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
    public override void OnAnimationEvent()
    {
        playerStateMachine.Combat.OnAttackAnimationEnd();
    }

    float attackCooldown;
    private Coroutine fireRoutine;
    Vector3 targetPos;
    float attack = 0;
    float speed = 1;
    bool isAttackTrigger = false;
    float maxAttack = 0.5f;
    Coroutine zoomRoutine;
    PlayerStateMachine playerStateMachine;
    PlayerRef me;
    NetworkObject magic { get; set; }
    private float fireDelay = 0.2f;
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
