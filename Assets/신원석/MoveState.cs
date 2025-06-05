using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;

public class MoveState : BaseState<PlayerStateMachine.PlayerState>
{

    public MoveState(PlayerStateMachine.PlayerState key , Animator animator,Transform transform,CinemachineCamera cinemachine) : base(key,animator)
    {
        cameraController = Camera.main.GetComponent<CameraController>();

        this.camera = cinemachine;

        this.transform = transform;
    }

    public override void EnterState()
    {
        animator.SetBool("Moving",true);
    }
    public override void ExitState()
    {

    }
    public override void UpdateState()
    {
        // ����, ���� �Է°� �ޱ�
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // ��ü �̵��� ���
        moveAmountZ = Mathf.Clamp01(Mathf.Abs(h));
        moveAmountY = Mathf.Clamp01(Mathf.Abs(v));

        // �Է� ���� ����ȭ
        var moveInput = (new Vector3(h, 0, v)).normalized;

        // ī�޶� ������ �������� �̵� ���� ���
        var moveDir = cameraController.PlanarRotation * moveInput;

        animator.SetFloat("Velocity Z", v);
        animator.SetFloat("Velocity Y", h);

        targetRotation = Quaternion.LookRotation(moveDir);

        Debug.Log($"Camera Y: {targetRotation}, Player Y: {transform.rotation}");

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

    }

    public override PlayerStateMachine.PlayerState GetNextState()
    {
      if(Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
      {
            animator.SetFloat("Velocity Z", 0);
            animator.SetBool("Moving", false);
            return PlayerStateMachine.PlayerState.Idle;
      }

        return PlayerStateMachine.PlayerState.Move;
    }

    public override void OnTriggerEnter(Collider collider) { }
    public override void OnTriggerExit(Collider collider) { }
    public override void OnTriggerStay(Collider collider) { }

    float moveAmountZ;
    float moveAmountY;
    CameraController cameraController;
    CinemachineCamera camera;

    [SerializeField] float rotationSpeed = 500f;
    Quaternion targetRotation;
    Transform transform;
}
