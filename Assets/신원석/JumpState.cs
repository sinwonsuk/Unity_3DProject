
using UnityEngine;


public class JumpState : BaseState<PlayerStateMachine.PlayerState>
{
    PlayerStateMachine playerStateMachine;
    private bool isGrounded;
    public Transform groundCheck;
    public float groundDistance = 0.2f;
    private Vector3 velocity;
    private float moveSpeed = 3;
    private LayerMask groundMask;

    public JumpState(PlayerStateMachine.PlayerState key, Animator animator,PlayerStateMachine playerStateMachine) : base(key, animator)
    {
        this.playerStateMachine = playerStateMachine;
        groundMask = playerStateMachine.groundMask;
        groundCheck = playerStateMachine.groundCheck;

    }

    private float gravity = -9.81f;
    private float jumpHeight = 2f;

    public override void EnterState()
    {
        animator.SetBool("Jump", true);
        velocity = Vector3.zero;

        // 점프 순간 속도 계산: sqrt(2gh)
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    public override void UpdateState()
    {      
        
    }



    public override void FixedUpdateState()
    {
        isGrounded = CheckGround();

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // 착지시 눌림 효과
            animator.SetBool("Jump", false);
            playerStateMachine.ChangeState(PlayerStateMachine.PlayerState.Idle);
            return;
        }

        // 이동 입력
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0, v);

        move = playerStateMachine.transform.TransformDirection(move);

        playerStateMachine.GetComponent<CharacterController>().Move(move * Time.fixedDeltaTime * moveSpeed);

        // 중력 적용
        velocity.y += gravity * Time.deltaTime;
        playerStateMachine.GetComponent<CharacterController>().Move(velocity * Time.fixedDeltaTime);

    }

    bool CheckGround()
    {
        return isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    public override PlayerStateMachine.PlayerState GetNextState()
    {     
        return PlayerStateMachine.PlayerState.Jump;
    }



    public override void OnTriggerEnter(Collider collider) { }
    public override void OnTriggerExit(Collider collider) { }
    public override void OnTriggerStay(Collider collider) { }

    public override void ExitState()
    {
        
    }

    public override void LateUpdateState(){}
}

