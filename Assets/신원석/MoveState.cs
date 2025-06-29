using Fusion;
using UnityEngine;
using UnityEngine.AI;
using static PlayerStateMachine;
using static Unity.Collections.Unicode;

public class MoveState : BaseState<PlayerStateMachine.PlayerState>
{
    PlayerStateMachine playerStateMachine;

    [SerializeField] float rotationSpeed = 500f;

    public MoveState(PlayerStateMachine.PlayerState key, PlayerStateMachine playerStateMachine)
        : base(key)
    {
        this.playerStateMachine = playerStateMachine;
    }

    public override void EnterState()
    {

    }

    public override void ExitState()
    {
        var animator = playerStateMachine.NetAnim.Animator;
        animator.SetFloat("MoveLeftRight", 0);
        animator.SetFloat("MoveForWard", 0);
        playerStateMachine.MoveSpeed = playerStateMachine.CurrentSpeed;
    }
    public void Move()
    {
        playerStateMachine.MoveInput();
    }

    public override void FixedUpdateState()
    {
        TryHandleRollInput();
        TryHandleJumpInput();
        TryHandleAttackInput();
        UpdateMovementAnimation();
        Move();
        ChangeIdle();
    }
    public override PlayerStateMachine.PlayerState GetNextState()
    {
        return PlayerStateMachine.PlayerState.Move;
    }
    public void ChangeIdle()
    {
        if (playerStateMachine.inputHandler.IsMove() == false)
        {
            if (playerStateMachine.Object.HasInputAuthority)
            {
                playerStateMachine.BroadcastIdleEvent(PlayerState.Idle);
                return;
            }
        }
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


        // 3) Weapon의 입력 권한자가 이 플레이어와 같다면 스킵
        if (weaponNetObj.InputAuthority == playerStateMachine.Object.InputAuthority)
            return;

        int attack = weaponNetObj.gameObject.GetComponent<WeaponNetworkObject>().weaponInfoConfig.Attack;

        playerStateMachine.health.RequestDamage(attack);

        playerStateMachine.BroadcastIdleEvent(PlayerState.Hit);
    }
    public override void OnTriggerExit(Collider collider) { }
    public override void OnTriggerStay(Collider collider) { }

    private void TryHandleRollInput()
    {
         if (playerStateMachine.IsWeapon == false && playerStateMachine.Stamina.currentStamina < playerStateMachine.RollStaminaCost)
            return;

        if (playerStateMachine.inputHandler.RollInput(out ERollState dir))
            Roll(dir);
    }
    private void Roll(ERollState dir)
    {
        playerStateMachine.NetAnim.Animator.SetInteger("RollCount", (int)dir);


        if (playerStateMachine.Stamina.currentStamina < playerStateMachine.RollStaminaCost)
            return;


        if (playerStateMachine.Object.HasStateAuthority)
        {
            playerStateMachine.SyncedState = PlayerState.Roll;
            playerStateMachine.RollCount = (int)dir;
        }         
        else
        {
            playerStateMachine.BroadcastIdleEvent(PlayerState.Roll);
            playerStateMachine.RPC_SetRollCount((int)dir);
        }
          
        return;
    }

    private bool TryHandleJumpInput()
    {

        float currentStamina = playerStateMachine.Stamina.currentStamina;
        float jumpCcost = playerStateMachine.JumpStaminaCost;


        if (playerStateMachine.inputHandler.IsCtrlButtonPress() && currentStamina > jumpCcost)
        {
            playerStateMachine.NetAnim.Animator.SetBool("Jump", true);

            playerStateMachine.playerController.Move(playerStateMachine.playerController.transform.position, 5);

            playerStateMachine.BroadcastIdleEvent(PlayerState.Jump);

            return true;
        }
        
        return false;
    }

    private void TryHandleAttackInput()
    {
        // 2) 자주 쓰는 값 로컬 변수에 할당
        var state = playerStateMachine;
        var input = state.inputHandler;
        bool hasInput = state.Object.HasInputAuthority;
        bool hasState = state.Object.HasStateAuthority;
        bool hasWeapon = state.IsWeapon;
        bool hasCamera = state.cameraManager.isCameraCheck;
        float stamina = state.Stamina.currentStamina;
        float cost = state.AttackStaminaCost;
        int count = state.AnimHandler.WeaponCount;

        // 3) 기본 가드: 무기 미소지, 입력권한, 스태미너 부족 시 중단
        if (!hasWeapon || !hasInput || stamina <= cost)
            return;

        // 4) 기본 공격 (근접)
        if (input.IsAttackPressed() &&
            count != (int)ItemState.Bow &&
            count != (int)ItemState.FireMagic)
        {
            if (hasState)
                state.SyncedState = PlayerState.Attack;
            else
                state.BroadcastIdleEvent(PlayerState.Attack);

            return;
        }

        // 5) 카메라 확인 후, 우클릭 공격 처리
        if (!hasCamera)
            return;

        if (input.IsRightAttackPressed())
        {
            if (count == (int)ItemState.Bow)
                state.BroadcastIdleEvent(PlayerState.BowAttack);
            else if (count == (int)ItemState.FireMagic)
                state.BroadcastIdleEvent(PlayerState.Magic);

            return;
        }
    }

    private void UpdateMovementAnimation()
    {

        // 1) 입력 가져오기 실패 시 조기 반환
        if (!playerStateMachine.GetInput(out NetworkInputData data))
            return;

        // 2) 자주 쓰는 값 로컬 변수에 할당
        PlayerStateMachine state = playerStateMachine;
        InputHandler input = state.inputHandler;
        float deltaTime = state.Runner.DeltaTime;
        PlayerStamina stamina = state.Stamina;
        float rawX = data.moveAxis.x;
        float rawZ = data.moveAxis.z;

        // 3) 좌/우 이동 스무딩
        state.moveX = Mathf.MoveTowards(state.moveX, rawX, deltaTime * 5f);

        // 4) 앞/뒤 이동: 입력값 직접 적용 후, 없을 땐 감속
        state.moveZ = rawZ;
        state.moveZ = Mathf.MoveTowards(state.moveZ, 0f, deltaTime * 5f);

        // 5) 달리기 여부 판단
        bool isRunning = input.IsShiftButtonPress()
                         && state.moveZ > 0f
                         && stamina.currentStamina > 0f;

        // 6) 달리기일 때 속도 및 스태미나 처리
        if (isRunning && stamina.currentStamina > 0.0f)
        {
            state.moveZ = Mathf.Lerp(0f, 2f, state.moveZ);
            state.MoveSpeed = state.MaxSpeed;
            stamina.ConsumeStaminaOnServer(deltaTime * 10.0f);
        }
        else
        {
            state.MoveSpeed = state.CurrentSpeed;

        }



            

        // 8) 애니메이터에 파라미터 전달
        var animator = state.NetAnim.Animator;
        animator.SetFloat("MoveLeftRight", state.moveX);
        animator.SetFloat("MoveForWard", state.moveZ);
    }

    public override void OnAnimationEvent()
    {

    }

   
}