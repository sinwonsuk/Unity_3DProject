using Cinemachine;
using Fusion;
using System.Collections;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
using static Unity.Collections.Unicode;


public class PlayerStateMachine : StageManager<PlayerStateMachine.PlayerState>
{
    // [Networked] private TickTimer delay { get; set; }
    public enum PlayerState
    {
        Idle,
        Move,
        Switch,
        Jump,
        Attack,
        Roll,
    }


    Animator animator;
    public PlayerCombat Combat { get; private set; }
    public AnimationHandler AnimHandler { get; private set; }

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


    private CinemachineVirtualCamera cam;

    public Transform groundCheck;
    public LayerMask groundMask;
    public bool isWeapon = false;
    public bool is3rdPersonCamera { get; set; }


    public NetworkCharacterController playerController {  get; private set; }
    private InputHandler inputHandler;

    public Vector3 rootMotionDelta { get; set; }
    public Quaternion rootMotionRotation { get; set; }

    public bool isRoll { get; set; } = false;
    bool isAttack = true;

    int hashAttackCount = Animator.StringToHash("AttackCount");

    public bool nextAttackQueued { get; set; } = false;
    public int AttackCount
    {
        get => animator.GetInteger(hashAttackCount);
        set => animator.SetInteger(hashAttackCount, value);
    }


    private void Awake()
    {

        animator = GetComponent<Animator>();
        playerController = GetComponent<NetworkCharacterController>();


        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 너무 비대해져서 역활 나눔 
        inputHandler = new InputHandler(this);
        Combat = new PlayerCombat(this);
        AnimHandler = new AnimationHandler(animator);

        GameObject camObj = GameObject.FindGameObjectWithTag("VirtualCam");
        cam = camObj.GetComponent<CinemachineVirtualCamera>();
        cam.Follow = cameraFollow;
        cam.LookAt = transform;

        GameObject camObj2 = GameObject.FindGameObjectWithTag("VirtualCam2");
        CinemachineVirtualCamera cam2 = camObj2.GetComponent<CinemachineVirtualCamera>();
        cam2.Follow = cameraFollow;
        cam2.LookAt = transform;

        states[PlayerState.Idle] = new IdleState(PlayerState.Idle,animator, this);
        states[PlayerState.Move] = new MoveState(PlayerState.Move, animator, this);
        states[PlayerState.Switch] = new WeaponSwitchState(PlayerState.Switch, animator, this);
        states[PlayerState.Attack] = new AttackState(PlayerState.Attack,animator, this);
        states[PlayerState.Roll] = new RollState(PlayerState.Roll, animator, this);
        states[PlayerState.Jump] = new JumpState(PlayerState.Jump, animator, this);

        currentState = states[PlayerState.Idle];
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
            attackState.OnAttackAnimationEnd();
        }
    }

    public bool RollInput()
    {
        if (!isWeapon)
            return false;

        if (inputHandler.RollInput(out ERollState dir))
        {
            Roll(dir);
            return true;
        }

        return false;
    }
    private void Roll(ERollState dir)
    {
        animator.SetInteger("RollCount", (int)dir);
        ChangeState(PlayerState.Roll);
    }

    public void MoveInput()
    {
        // 2. 입력 처리 (XZ)
        inputHandler.TryGetMoveDirection(out Vector3 moveDir, out Quaternion planarRotation);

        // 3. 회전 처리
        if (Object.HasInputAuthority)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                planarRotation,
                Time.fixedDeltaTime * rotationSpeed
            );

            // 4. Y축 이동값 직접 추가
            Vector3 moveWithGravity = moveDir * moveSpeed;

            playerController.Move(moveWithGravity * Runner.DeltaTime);
        }
    }

    public bool ComboAttackInput()
    {
        if (isWeapon == false)
            return false;

        if (inputHandler.IsAttackPressed() == true)
        {
            animator.SetBool("Attack", true);
            ChangeState(PlayerState.Attack);
            return true;
        }

        return false;
    }

    public bool DashAttackInput()
    {
        if (isWeapon == false)
            return false;

        if (inputHandler.IsDashAttackPressed())
        {
            animator.SetBool("RunAttack", true);
            ChangeState(PlayerState.Attack);
            return true;
        }

        return false;
    }

    public void AttackMove()
    {
        if (isAttack == false)
            return;

        playerController.Move(transform.forward * Runner.DeltaTime * attackSpeed);
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

    public void StopRoll()
    {
        isRoll = true;
    }
    public void startRoll()
    {
        isRoll = false;
    }
    public void SetIsAttackTrue()
    {
        isAttack = true;
    }
    public void SetIsAttackFalse()
    {
        isAttack = false;
    }  
}
