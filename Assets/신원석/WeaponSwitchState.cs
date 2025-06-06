using UnityEngine;
using UnityEngine.AI;

public class WeaponSwitchState : BaseState<PlayerStateMachine.PlayerState>
{

    public WeaponSwitchState(PlayerStateMachine.PlayerState key, Animator animator, Transform transform) : base(key, animator)
    {
        cameraController = Camera.main.GetComponent<CameraController>();

        this.transform = transform;
    }

    public override void EnterState()
    {
        animator.SetTrigger("KatanaEquip");
        

        EventBus<EquipWeaponEvent>.Raise(new EquipWeaponEvent(1));
    }
    public override void ExitState()
    {

    }
    public override void UpdateState()
    {
        // 수평, 수직 입력값 받기
        //float h = Input.GetAxis("Horizontal");
        //float v = Input.GetAxis("Vertical");

        //// 전체 이동량 계산
        //moveAmount = Mathf.Clamp01(Mathf.Abs(h) + Mathf.Abs(v));

        //// 입력 방향 정규화
        //var moveInput = (new Vector3(h, 0, v)).normalized;

        //// 카메라 방향을 기준으로 이동 방향 계산
        ////var moveDir = cameraController.PlanarRotation * moveInput;

        //animator.SetFloat("Velocity Z", moveAmount);

        ////targetRotation = Quaternion.LookRotation(moveDir);

        //Debug.Log($"Camera Y: {targetRotation}, Player Y: {transform.rotation}");

        //transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

    }

    public override PlayerStateMachine.PlayerState GetNextState()
    {
        //if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
        //{
        //    animator.SetFloat("Velocity Z", 0);
        //    animator.SetBool("Moving", false);
        //    return PlayerStateMachine.PlayerState.Idle;
        //}

        return PlayerStateMachine.PlayerState.Idle;
    }

    public override void OnTriggerEnter(Collider collider) { }
    public override void OnTriggerExit(Collider collider) { }
    public override void OnTriggerStay(Collider collider) { }

    public override void FixedUpdateState(){ }

    float moveAmount;
    CameraController cameraController;
    [SerializeField] float rotationSpeed = 500f;
    Quaternion targetRotation;
    Transform transform;
}
