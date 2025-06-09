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

    // 카메라 회전 속도

    // 카메라와 타겟 사이의 거리
    [SerializeField] float distance = 5;

    // 수직 회전 제한 각도
    [SerializeField] float minVerticalAngle = -45;
    [SerializeField] float maxVerticalAngle = 45;

    // 카메라 위치 미세 조정을 위한 오프셋
    [SerializeField] Vector2 framingOffset;

    // 마우스 X축 반전 여부
    [SerializeField] bool invertX;
    // 마우스 Y축 반전 여부
    [SerializeField] bool invertY;

    // 현재 X축 회전값 (상하 회전)
    float rotationX;
    // 현재 Y축 회전값 (좌우 회전)
    float rotationY;

    // X축 반전 계수
    float invertXVal;
    // Y축 반전 계수
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
        // 컴포넌트 연결 
        animator = GetComponent<Animator>();
        playerController = GetComponent<NetworkCharacterController>();


        // 인풋시스템 따로 관리 
        inputHandler = new InputHandler(this);

        //시네마신 카메라 연결 
        GameObject camObj = GameObject.FindGameObjectWithTag("VirtualCam");
        cam = camObj.GetComponent<CinemachineVirtualCamera>();
        cam.Follow = transform;
        cam.LookAt = transform;

        // 각 상태 객체 생성 및 등록
        states[PlayerState.Idle] = new IdleState(PlayerState.Idle,animator, this);
        states[PlayerState.Move] = new MoveState(PlayerState.Move, animator, this);
        states[PlayerState.Switch] = new WeaponSwitchState(PlayerState.Switch, animator, this);
        states[PlayerState.Attack] = new AttackState(PlayerState.Attack,animator, this);
        states[PlayerState.Roll] = new RollState(PlayerState.Roll, animator, this);
        states[PlayerState.Jump] = new JumpState(PlayerState.Jump, animator, this);

        // 초기 상태 설정
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
        // 마우스 반전 설정에 따른 계수 설정
        invertXVal = (invertX) ? -1 : 1;
        invertYVal = (invertY) ? -1 : 1;

        // 마우스 Y축 입력으로 상하 회전 처리
        rotationX += Input.GetAxis("Mouse Y") * invertYVal * rotationSpeed;
        rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);

        // 마우스 X축 입력으로 좌우 회전 처리
        rotationY += Input.GetAxis("Mouse X") * invertXVal * rotationSpeed;

        transform.rotation = Quaternion.Euler(0, rotationY * Runner.DeltaTime, 0);


        //// 최종 회전값 계산
        //var targetRotation = Quaternion.Euler(rotationX, rotationY, 0);

        //// 카메라 위치 및 회전 적용
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
