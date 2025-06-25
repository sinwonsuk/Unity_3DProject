
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

    public JumpState(PlayerStateMachine.PlayerState key,PlayerStateMachine playerStateMachine) : base(key)
    {
        this.playerStateMachine = playerStateMachine;
        groundMask = playerStateMachine.groundMask;
        groundCheck = playerStateMachine.groundCheck;

    }

    private float gravity = 9.81f;
    private float jumpHeight = 2f;

    public override void EnterState()
    {
        playerStateMachine.NetAnim.Animator.SetBool("Jump", true);

        float v0 = Mathf.Sqrt(2f * gravity * jumpHeight);

        playerStateMachine.playerController.Move(Vector3.zero, v0);

    }
    public override void ExitState()
    {
        playerStateMachine.NetAnim.Animator.SetBool("Jump", false);
    }

    public override void FixedUpdateState()
    {
        if (!playerStateMachine.Object.HasStateAuthority)
            return;


        if (playerStateMachine.playerController.IsGrounded == true)
        {
            playerStateMachine.NetAnim.Animator.SetBool("Jump", false);
            playerStateMachine.SyncedState = PlayerStateMachine.PlayerState.Idle;
            return;
        }
        else if (playerStateMachine.playerController.IsGrounded ==true)
        {
            playerStateMachine.NetAnim.Animator.SetBool("Jump", false);
            playerStateMachine.RPC_BroadcastState(PlayerStateMachine.PlayerState.Idle);
            return;
        }

        Vector3 kinematicVel = Vector3.zero;

        if (playerStateMachine.GetInput(out NetworkInputData data))
        {
            Vector3 dir = new Vector3(data.direction.x, 0, data.direction.z).normalized;
            kinematicVel = playerStateMachine.transform.TransformDirection(dir * moveSpeed);
        }

        playerStateMachine.playerController.Move(kinematicVel, 0f);
    }

    public override PlayerStateMachine.PlayerState GetNextState()
    {     
        return PlayerStateMachine.PlayerState.Jump;
    }



    public override void OnTriggerEnter(Collider collider) { }
    public override void OnTriggerExit(Collider collider) { }
    public override void OnTriggerStay(Collider collider) { }

 
    public override void OnHitAnimationEvent()
    {

    }
}

