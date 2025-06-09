using Cinemachine;
using Fusion;
using System;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem.XR;
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

    // ī�޶� ȸ�� �ӵ�

    // ī�޶�� Ÿ�� ������ �Ÿ�
    [SerializeField] float distance = 5;

    // ���� ȸ�� ���� ����
    [SerializeField] float minVerticalAngle = -45;
    [SerializeField] float maxVerticalAngle = 45;

    // ī�޶� ��ġ �̼� ������ ���� ������
    [SerializeField] Vector2 framingOffset;

    // ���콺 X�� ���� ����
    [SerializeField] bool invertX;
    // ���콺 Y�� ���� ����
    [SerializeField] bool invertY;

    // ���� X�� ȸ���� (���� ȸ��)
    float rotationX;
    // ���� Y�� ȸ���� (�¿� ȸ��)
    float rotationY;

    // X�� ���� ���
    float invertXVal;
    // Y�� ���� ���
    float invertYVal;

    Animator animator;

    [SerializeField]
    float moveSpeed = 5.0f;
    [SerializeField]
    float rotationSpeed = 2.0f;

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


        // ��ǲ�ý��� ���� ���� 
        inputHandler = new InputHandler(this);

        //�ó׸��� ī�޶� ���� 
        GameObject camObj = GameObject.FindGameObjectWithTag("VirtualCam");
        cam = camObj.GetComponent<CinemachineVirtualCamera>();
        cam.Follow = transform;
        cam.LookAt = transform;

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
    public void test()
    {
        // ���콺 ���� ������ ���� ��� ����
        invertXVal = (invertX) ? -1 : 1;
        invertYVal = (invertY) ? -1 : 1;

        // ���콺 Y�� �Է����� ���� ȸ�� ó��
        rotationX += Input.GetAxis("Mouse Y") * invertYVal * rotationSpeed;
        rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);

        // ���콺 X�� �Է����� �¿� ȸ�� ó��
        rotationY += Input.GetAxis("Mouse X") * invertXVal * rotationSpeed;

        transform.rotation = Quaternion.Euler(0, rotationY * Runner.DeltaTime, 0);


        //// ���� ȸ���� ���
        //var targetRotation = Quaternion.Euler(rotationX, rotationY, 0);

        //// ī�޶� ��ġ �� ȸ�� ����
        //var focusPosition = new Vector3(framingOffset.x, framingOffset.y, 0);
        //transform.position = focusPosition - targetRotation * new Vector3(0, 0, distance);
        //transform.rotation = targetRotation;
    }

    public void MoveInput()
    {
        test();

        //if (inputHandler.TryGetMoveDirection(out Vector3 dir, out Quaternion planarRotation))
        //{
        //    transform.rotation = Quaternion.RotateTowards(transform.rotation, planarRotation, rotationSpeed * Runner.DeltaTime);
        //    playerController.Move(dir * Runner.DeltaTime);
        //}
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
