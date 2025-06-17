using Cinemachine;
using Fusion;
using UnityEngine;


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


    public WeaponsConfig weapons;

    public ItemState Item { get; set; } = ItemState.none;


    Animator animator;
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
    public LayerMask LayerMask
    {
        get => layerMask;
        set => layerMask = value;
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

    public override void Spawned()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponent<NetworkCharacterController>();



        states[PlayerState.Idle] = new IdleState(PlayerState.Idle, animator, this);
        states[PlayerState.Move] = new MoveState(PlayerState.Move, animator, this);
        states[PlayerState.Switch] = new WeaponSwitchState(PlayerState.Switch, animator, this);
        states[PlayerState.Attack] = new AttackState(PlayerState.Attack, animator, this);
        states[PlayerState.Roll] = new RollState(PlayerState.Roll, animator, this);
        states[PlayerState.BowAttack] = new BowState(PlayerState.BowAttack, animator, this);
        states[PlayerState.Jump] = new JumpState(PlayerState.Jump, animator, this);
        states[PlayerState.Magic] = new MagicAttackState(PlayerState.Magic, animator, this);
        currentState = states[PlayerState.Idle];



        if (Object.HasInputAuthority)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;
            GameObject camObj = GameObject.FindGameObjectWithTag("VirtualCam2");
            Cam = camObj.GetComponent<CinemachineVirtualCamera>();
            Cam.Follow = cameraFollow;
            Cam.LookAt = cameraFollow;
        }

        // �ʹ� ��������� ��Ȱ ���� 
        inputHandler = new InputHandler(this, CameraFollow);
        Combat = new PlayerCombat(this);
        AnimHandler = new AnimationHandler(animator);
        WeaponManager = new WeaponManager(weapons, rightHandTransform, leftHandTransform);
        cameraManager = new CameraManager(Cam);
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
  

        if (inputHandler.TryGetMoveDirection(out Vector3 moveDir, out Quaternion planarRot))
        {
            Debug.DrawRay(transform.position, moveDir * 2f, Color.green, 0.1f);
            Debug.Log("�̵� ���� ������");
            playerController.Move(moveDir * moveSpeed * Runner.DeltaTime);
        }
        else
        {
            Debug.Log("�Է� �����ų� 0����");
        }
    }

    //public override void FixedUpdateNetwork()
    //{
    //    if (GetInput(out NetworkInputData data))
    //    {
    //        data.direction.Normalize();
    //        playerController.Move(5 * data.direction * Runner.DeltaTime);

    //    }
    //}

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
