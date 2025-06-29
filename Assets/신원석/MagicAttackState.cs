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

        float attackStamina = playerStateMachine.AttackStaminaCost;
        float CutrrentStanmina = playerStateMachine.Stamina.currentStamina;


        if (left && right && attack >= maxAttack && playerStateMachine.cameraManager.isCameraCheck == true && 
            isAttackTrigger ==false && attackStamina < CutrrentStanmina)
        {
            // 발사 위치 계산
            CalculateHitPosition();

            playerStateMachine.Stamina.UseStamina(attackStamina);
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
