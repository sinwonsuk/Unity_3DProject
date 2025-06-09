using Cinemachine;
using Fusion;
using UnityEngine;


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

    [SerializeField]
    float moveSpeed = 5.0f;
    [SerializeField]
    float rotationSpeed = 2.0f;
    [SerializeField]
    Transform cameraFollow;



    private CinemachineVirtualCamera cam;

    public Transform groundCheck;
    public LayerMask groundMask;
    public bool isWeapon = false;
    public bool is3rdPersonCamera { get; set; }


    public NetworkCharacterController playerController {  get; private set; }
    private InputHandler inputHandler;

    

    private void Awake()
    {
        // ������Ʈ ���� 
        animator = GetComponent<Animator>();
        playerController = GetComponent<NetworkCharacterController>();

        // ���콺 Ŀ�� 
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;


        // ��ǲ�ý��� ���� ���� 
        inputHandler = new InputHandler(this);

        //�ó׸��� ī�޶� ���� 
        GameObject camObj = GameObject.FindGameObjectWithTag("VirtualCam");
        cam = camObj.GetComponent<CinemachineVirtualCamera>();
        cam.Follow = cameraFollow;
        cam.LookAt = transform;

        GameObject camObj2 = GameObject.FindGameObjectWithTag("VirtualCam2");
        CinemachineVirtualCamera cam2 = camObj2.GetComponent<CinemachineVirtualCamera>();
        cam2.Follow = cameraFollow;
        cam2.LookAt = transform;


        // �� ���� ��ü ���� �� ���
        states[PlayerState.Idle] = new IdleState(PlayerState.Idle,animator, this);
        states[PlayerState.Move] = new MoveState(PlayerState.Move, animator, this);
        states[PlayerState.Switch] = new WeaponSwitchState(PlayerState.Switch, animator, this);
        states[PlayerState.Attack] = new AttackState(PlayerState.Attack,animator, this);
        states[PlayerState.Roll] = new RollState(PlayerState.Roll, animator, this);
        states[PlayerState.Jump] = new JumpState(PlayerState.Jump, animator, this);

        // �ʱ� ���� ����
        currentState = states[PlayerState.Idle];
    }

    private void OnEnable()
    {

    }
    private void OnDisable()
    {
       
    }
   

    public void MoveInput()
    {
        inputHandler.TryGetMoveDirection(out Vector3 moveDir,out Quaternion planarRotation);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, planarRotation, Time.fixedDeltaTime * rotationSpeed);
        playerController.Move(moveDir * Time.fixedDeltaTime * moveSpeed);

    }
    public void CameraInput()
    {
        if (inputHandler.ChangeCamera())
        {
            is3rdPersonCamera = !is3rdPersonCamera;
        }
    }


    //public override void FixedUpdateNetwork()
    //{
    //    if (GetInput(out NetworkInputData data))
    //    {
    //        data.direction.Normalize();
    //        playerController.Move(5 * data.direction * Runner.DeltaTime);


    //        if (data.direction.sqrMagnitude > 0)
    //            _forward = data.direction;

    //        if (HasStateAuthority && delay.ExpiredOrNotRunning(Runner))
    //        {
    //            if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0))
    //            {
    //                delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
    //            }
    //        }
    //    }
    //}









}
