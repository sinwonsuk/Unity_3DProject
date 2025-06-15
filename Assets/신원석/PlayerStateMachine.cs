using Cinemachine;
using Fusion;
using System.Collections;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
using static Unity.Collections.Unicode;
using static UnityEngine.InputSystem.OnScreen.OnScreenStick;

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
    // [Networked] private TickTimer delay { get; set; }
    public enum PlayerState
    {
        Idle,
        Move,
        Switch,
        Jump,
        Attack,
        Roll,
        BowAttack,
    }


    public WeaponsConfig weapons;

    public ItemState Item { get; set; } = ItemState.none;


    Animator animator;
    public PlayerCombat Combat { get; private set; }
    public AnimationHandler AnimHandler { get; private set; }
    public InputHandler inputHandler { get; private set; }

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


    Quaternion targetRotation;
    Vector3 targetMove;

    public Transform RightHandTransform
    {
        get => rightHandTransform;
        set => rightHandTransform = value;
    }
    public Transform CameraFollow
    {
        get => cameraFollow;
        set => cameraFollow = value;
    }

    public CinemachineVirtualCamera Cam { get; set; }

    public Transform groundCheck;
    public LayerMask groundMask;
    public bool isWeapon = false;
    public bool is3rdPersonCamera { get; set; }


    public NetworkCharacterController playerController {  get; private set; }
   

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
        Cursor.visible = true;

        // 너무 비대해져서 역활 나눔 
        inputHandler = new InputHandler(this, CameraFollow);
        Combat = new PlayerCombat(this);
        AnimHandler = new AnimationHandler(animator);
        WeaponManager = new WeaponManager(weapons,rightHandTransform, leftHandTransform);

        //GameObject camObj = GameObject.FindGameObjectWithTag("VirtualCam");
        //Cam = camObj.GetComponent<CinemachineVirtualCamera>();
        //Cam.Follow = cameraFollow;
        //Cam.LookAt = transform;

        GameObject camObj2 = GameObject.FindGameObjectWithTag("VirtualCam2");
        CinemachineVirtualCamera cam2 = camObj2.GetComponent<CinemachineVirtualCamera>();
        cam2.Follow = cameraFollow;
        cam2.LookAt = cameraFollow;

        states[PlayerState.Idle] = new IdleState(PlayerState.Idle,animator, this);
        states[PlayerState.Move] = new MoveState(PlayerState.Move, animator, this);
        states[PlayerState.Switch] = new WeaponSwitchState(PlayerState.Switch, animator, this);
        states[PlayerState.Attack] = new AttackState(PlayerState.Attack,animator, this);
        states[PlayerState.Roll] = new RollState(PlayerState.Roll, animator, this);
        states[PlayerState.BowAttack] = new BowState(PlayerState.BowAttack, animator, this);
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
    private Tick _initial;
    public void MoveInput()
    {

        //if (!Object.HasInputAuthority)
        //    return;



        if (!Object.HasInputAuthority) return;

        // 카메라 기준 이동 direction

        inputHandler.TryGetMoveDirection(out Vector3 _moveDir, out Quaternion planarRot);


        playerController.Move(_moveDir);


       



        //inputHandler.TryGetMoveDirection(out Vector3 _moveDir, out Quaternion planarRot);

       // // 시뮬레이션 목표치 결정
       // targetMove = _moveDir;
       // targetRotation = planarRot;


       // transform.rotation = Quaternion.RotateTowards(
       //transform.rotation,
       //targetRotation,
       //rotationSpeed * Time.deltaTime);


       // // 결정론 보장된 물리 이동
        //playerController.Move(move);
    }

    public void testte()
    {
       //if (!Object.HasInputAuthority)
       //    return;

       // // 부드러운 회전 보간
       // transform.rotation = Quaternion.RotateTowards(
       // transform.rotation,
       // targetRotation,
       // rotationSpeed * Time.deltaTime)
       //;
    }

    public void RotatePlayer()
    {
        //if (Object.HasInputAuthority)
        //{
        //    Quaternion quaternion = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);

        //    transform.rotation = Quaternion.RotateTowards(
        //        transform.rotation,
        //        quaternion,
        //        Runner.DeltaTime * rotationSpeed
        //    );
        //    // 4. Y축 이동값 직접 추가

        //}
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
