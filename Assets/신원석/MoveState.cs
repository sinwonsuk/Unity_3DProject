using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;


public class MoveState : BaseState<PlayerStateMachine.PlayerState>
{
    PlayerStateMachine PlayerStateMachine;

    public MoveState(PlayerStateMachine.PlayerState key ,Animator animator ,PlayerStateMachine playerStateMachine) : base(key,animator)
    {
        this.PlayerStateMachine = playerStateMachine;


        cameraController = Camera.main.GetComponent<CameraController>();
    }

    public override void EnterState()
    {
        animator.SetBool("Walk",true);
    }
    public override void ExitState()
    {
        animator.SetBool("Walk", false);
        animator.SetBool("Run", false);
        animator.SetBool("RightMove", false);
        animator.SetBool("LeftMove", false);
        moveSpeed = 2.0f;
    }
    public override void UpdateState()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (Input.GetKey(KeyCode.W))
            {
                animator.SetInteger("RollCount", (int)ERollState.Forward);
                PlayerStateMachine.ChangeState(PlayerStateMachine.PlayerState.Roll);
                return;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                animator.SetInteger("RollCount", (int)ERollState.Backward);
                PlayerStateMachine.ChangeState(PlayerStateMachine.PlayerState.Roll);
                return;
            }
            else if (Input.GetKey(KeyCode.A))
            {
                animator.SetInteger("RollCount", (int)ERollState.Left);
                PlayerStateMachine.ChangeState(PlayerStateMachine.PlayerState.Roll);
                return;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                animator.SetInteger("RollCount", (int)ERollState.Right);
                PlayerStateMachine.ChangeState(PlayerStateMachine.PlayerState.Roll);
                return;
            }
        }
    }

    public override void FixedUpdateState()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            animator.SetBool("Run", true);
            moveSpeed = 4.0f;
        }
        else
        {
            animator.SetBool("Run", false);
            moveSpeed = 2.0f;
        }

        if (Input.GetKey(KeyCode.D))
        {
            animator.SetBool("RightMove", true);
        }
        else
        {
            animator.SetBool("RightMove", false);
        }

        if (Input.GetKey(KeyCode.A))
        {
            animator.SetBool("LeftMove", true);
        }
        else
        {
            animator.SetBool("LeftMove", false);
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        float moveAmount = Mathf.Clamp01(Mathf.Abs(h) + Mathf.Abs(v));

        Vector3 moveInput = new Vector3(h, 0, v).normalized;

        // 카메라의 현재 수평(Y) 회전만 가져와서 적용
        float yaw = Camera.main.transform.eulerAngles.y;
        Quaternion planarRotation = Quaternion.Euler(0, yaw, 0);

        Vector3 moveDir = planarRotation * moveInput;

        var velocity = moveDir * moveSpeed;

      
        targetRotation = Quaternion.LookRotation(moveDir);
        PlayerStateMachine.transform.rotation = Quaternion.RotateTowards(PlayerStateMachine.transform.rotation, planarRotation, rotationSpeed * Time.deltaTime);

        PlayerStateMachine.GetComponent<CharacterController>().Move(velocity * Time.deltaTime);
       
    }



    public override PlayerStateMachine.PlayerState GetNextState()
    {
      if(Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
            return PlayerStateMachine.PlayerState.Idle;
        if (Input.GetMouseButtonDown(0))
            return PlayerStateMachine.PlayerState.Attack;


        return PlayerStateMachine.PlayerState.Move;
    }

    public override void OnTriggerEnter(Collider collider) { }
    public override void OnTriggerExit(Collider collider) { }
    public override void OnTriggerStay(Collider collider) { }

    float moveSpeed = 2.0f;


    CameraController cameraController;
    CinemachineCamera camera;

    [SerializeField] float rotationSpeed = 500f;
    Quaternion targetRotation;
}
