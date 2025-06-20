using Fusion;
using Cinemachine;
using UnityEngine;
using static UnityEngine.InputSystem.OnScreen.OnScreenStick;
using System;
using UnityEngine.InputSystem.XR;


public enum ItemState
{
    none,
    Sword,
    Harberd,
    Bow,
    Magic,
    Position,
    Arrow,
}


public class PlayerStateMachine : StageManager<PlayerStateMachine.PlayerState>
{
    [Networked] public PlayerState SyncedState { get; set; }
    [Networked] public bool comboAnimEnded { get; set; } = false;

    [Networked] public int AttackCount { get; set; } = 0;
    public NetworkMecanimAnimator NetAnim { get; set; }

    public enum PlayerState
    {
        Idle,
        Move,
        Switch,
        Jump,
        Attack,
        Roll,
        BowAttack,
        Magic,
    }
    public Action action;

    public WeaponsConfig weapons;
    public ItemState Item { get; set; } = ItemState.none;

    private Animator animator;
    public PlayerCombat Combat { get; private set; }
    public AnimationHandler AnimHandler { get; private set; }
    public InputHandler inputHandler { get; private set; }

    public CameraManager cameraManager { get; private set; }
    public WeaponManager WeaponManager { get; private set; }

    [SerializeField]
    float moveSpeed = 5.0f;
    [SerializeField]
    float rotationSpeed = 2.0f;
    [SerializeField]
    Transform cameraFollow;
    [SerializeField]
    float rollSpeed = 15.0f;
    [SerializeField]
    float attackSpeed = 2.0f;
    [SerializeField]
    Transform rightHandTransform;
    [SerializeField]
    Transform leftHandTransform;
    [SerializeField]
    LayerMask layerMask;

    public float AttackSpeed
    {
        get => attackSpeed;
        set => attackSpeed = value;
    }
    public Transform RightHandTransform
    {
        get => rightHandTransform;
        set => rightHandTransform = value;
    }
    public Transform LeftHandTransform
    {
        get => leftHandTransform;
        set => leftHandTransform = value;
    }
    public Transform CameraFollow
    {
        get => cameraFollow;
        set => cameraFollow = value;
    }
    public LayerMask LayerMask
    {
        get => layerMask;
        set => layerMask = value;
    }
    public CinemachineVirtualCamera Cam { get; set; }

    public Transform groundCheck;
    public LayerMask groundMask;
   [Networked] public bool IsWeapon { get; set; } = false;
   
    public NetworkCharacterController playerController {  get; private set; }
   
    public Vector3 rootMotionDelta { get; set; }
    public Quaternion rootMotionRotation { get; set; }
    public bool isRoll { get; set; } = false;
    [Networked] public bool isAttack { get; set; } = true;

    public bool nextAttackQueued { get; set; } = false;
 
    bool _isInitialized = false;
    public override void Spawned()
    {
        NetAnim = GetComponent<NetworkMecanimAnimator>();
        animator = GetComponent<Animator>();
        playerController = GetComponent<NetworkCharacterController>();

        states[PlayerState.Idle] = new IdleState(PlayerState.Idle, this);
        states[PlayerState.Move] = new MoveState(PlayerState.Move, this);
        states[PlayerState.Switch] = new WeaponSwitchState(PlayerState.Switch, this);
        states[PlayerState.Attack] = new AttackState(PlayerState.Attack, this);
        states[PlayerState.Roll] = new RollState(PlayerState.Roll, animator, this);
        states[PlayerState.BowAttack] = new BowState(PlayerState.BowAttack, animator, this);
        states[PlayerState.Jump] = new JumpState(PlayerState.Jump, this);
        states[PlayerState.Magic] = new MagicAttackState(PlayerState.Magic, animator, this);


        if (Object.HasStateAuthority)
            SyncedState = PlayerState.Idle;

        currentState = states[SyncedState];

        if (Object.HasInputAuthority)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;
            GameObject camObj = GameObject.FindGameObjectWithTag("VirtualCam2");
            Cam = camObj.GetComponent<CinemachineVirtualCamera>();
            Cam.Follow = cameraFollow;
            Cam.LookAt = cameraFollow;
        }

        // 너무 비대해져서 역활 나눔 
        inputHandler = new InputHandler(this, CameraFollow);
        Combat = new PlayerCombat(this);
        AnimHandler = new AnimationHandler(NetAnim);
        WeaponManager = GetComponent<WeaponManager>();
        WeaponManager.Init(weapons, rightHandTransform, leftHandTransform, Runner, this);
        cameraManager = new CameraManager(Cam);
        action = adad;
        _isInitialized = true;
    }
  
    private void OnEnable()
    {

    }
    private void OnDisable()
    {
       
    }

    public void OnAnimationEnd()
    {
        if (currentState is BaseState<PlayerState> attackState)
        {
            if (!Object.HasStateAuthority) return;
            comboAnimEnded = true;

        }
    }
    public void MoveInput()
    {
        if (inputHandler.IsMove() == true)
        {
            if (Object.HasInputAuthority && !Object.HasStateAuthority)
            {              
               MoveAndRotate(inputHandler.GetNetworkInputData());
            }
            else if (Object.HasStateAuthority)
            {
               MoveAndRotate(inputHandler.GetNetworkInputData());
            }
        }
    }

    public void MoveAndRotate(NetworkInputData data)
    {
        Vector3 moveInput = data.direction.normalized;
        Quaternion planarRot = Quaternion.Euler(0, data.CameraRotateY, 0);
        Vector3 moveDir = planarRot * moveInput;

        playerController.Move(moveDir * moveSpeed * Runner.DeltaTime);
        playerController.Rotate(planarRot);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_BroadcastState(PlayerState next, RpcInfo info = default)
    {
        if (!Object.HasStateAuthority) return;

        if (SyncedState == next) return;
        SyncedState = next;
    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void Rpc_SetWeapon(bool hasWeapon, RpcInfo info = default)
    {
        if (!Object.HasStateAuthority) return;

        IsWeapon = hasWeapon; 
    }



    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void Rpc_EndAttack(RpcInfo info = default)
    {
        AnimHandler.SetAttackBool(false);

    }

    public void SetWeapon(bool hasWeapon)
    {
        if (Object.HasInputAuthority)
        {
            Rpc_SetWeapon(hasWeapon);
        }
        if(Object.HasStateAuthority)
        {
            IsWeapon = hasWeapon;  // 이 한 줄이면 모든 클라이언트 동기화
        }

    }

    public void BroadcastIdleEvent(PlayerState nextStateKey)
    {
        if (Object.HasStateAuthority)
        {
            SyncedState = nextStateKey;
        }
        else if(Object.HasInputAuthority)
        {
            RPC_BroadcastState(nextStateKey);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!_isInitialized) return;

        string who = Object.HasInputAuthority && Runner.LocalPlayer == Object.InputAuthority
        ? "내 플레이어"
        : "다른 플레이어";

        Debug.Log($"[{who}] ObjID={Object.Id} InputAuth={Object.HasInputAuthority} StateAuth={Object.HasStateAuthority} SyncedState={SyncedState}");


        if (Object.HasStateAuthority)
        {
            var next = currentState.GetNextState();
            if (next != currentState.StateKey)
            {
                SyncedState = next;
            }        
        }
        // (3) 공통: 동기화된 상태 반영
        if (SyncedState != currentState.StateKey)
        {
            currentState.ExitState();
            currentState = states[SyncedState];
            currentState.EnterState();
        }
        // (4) 공통: 상태별 행동 실행
        currentState.FixedUpdateState();

        // (5) 이벤트 실행

    }
    public void adad()
    {
        if (currentState is BaseState<PlayerState> attackState && comboAnimEnded == true)
        {
            attackState.OnAttackAnimationEnd();
            comboAnimEnded = false;
        }
    }

    public void ComboAttackInput()
    {
        if (IsWeapon == false)
            return;

        if (inputHandler.IsAttackPressed() == true && Object.HasInputAuthority)
        {
            if (Object.HasStateAuthority)
            {
                // 호스트 자신은 직접 상태 변경
                SyncedState = PlayerState.Attack;
            }
            else
            {
                // 클라이언트는 RPC 요청
                RPC_BroadcastState(PlayerState.Attack);
            }

        }
    }

    public bool DashAttackInput()
    {
        if (IsWeapon == false)
            return false;

        if (inputHandler.IsDashAttackPressed())
        {
            NetAnim.Animator.SetBool("RunAttack", true);
            RPC_BroadcastState(PlayerState.Attack);
            return true;
        }

        return false;
    }

    public void MoveRoll(int count)
    {
        if (isRoll == true)
            return;

        if (ERollState.Backward == (ERollState)count)
        {
            playerController.Move(-transform.forward * Runner.DeltaTime * rollSpeed);
        }
        else if(ERollState.Forward == (ERollState)count)
        {
            playerController.Move(transform.forward * rollSpeed * Runner.DeltaTime);
        }
        else if(ERollState.Left == (ERollState)count)
        {
            playerController.Move(-transform.right * rollSpeed * Runner.DeltaTime);
        }
        else
        {
            playerController.Move(transform.right * rollSpeed * Runner.DeltaTime);
        }
    }

    public void StopRoll() => isRoll = true;
    public void startRoll() => isRoll = false;
    public void SetIsAttackTrue() => isAttack = true;
    public void SetIsAttackFalse() => isAttack = false;

    public override void Render()
    {
        //AnimHandler.SetAttackCount(AttackCount);  
        //AnimHandler.SetAttackBool(IsAttacking);   
    }

}
