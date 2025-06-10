using UnityEngine;
using UnityEngine.AI;
using static Unity.Collections.Unicode;
using static UnityEngine.Rendering.DebugUI;
using Fusion;
public class MoveState : BaseState<PlayerStateMachine.PlayerState>
{
    PlayerStateMachine PlayerStateMachine;

    [SerializeField] float rotationSpeed = 500f;
    float moveSpeed = 2.0f;

    public MoveState(PlayerStateMachine.PlayerState key, Animator animator, PlayerStateMachine playerStateMachine)
        : base(key, animator)
    {
        this.PlayerStateMachine = playerStateMachine;
    }

    public override void EnterState()
    {
        animator.SetBool("Walk", true);
    }

    public override void ExitState()
    {
        animator.SetFloat("MoveLeftRight", 0.0f);
        animator.SetFloat("MoveForWard", 0.0f);
        moveSpeed = 2.0f;
    }

    public override void UpdateState()
    {
        Debug.Log("Move");
        


        if (TryHandleJumpInput()) return;

        UpdateMovementAnimation();
    }

    public override void FixedUpdateState()
    {
        PlayerStateMachine.MoveInput();


        if (TryHandleRollInput()) return;
        TryHandleAttackInput();

       
    }
    public override void LateUpdateState()
    {
        
    }
    public override PlayerStateMachine.PlayerState GetNextState()
    {
        if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
            return PlayerStateMachine.PlayerState.Idle;

        return PlayerStateMachine.PlayerState.Move;
    }

    public override void OnTriggerEnter(Collider collider) { }
    public override void OnTriggerExit(Collider collider) { }
    public override void OnTriggerStay(Collider collider) { }

    private bool TryHandleRollInput()
    {
        return PlayerStateMachine.RollInput();
    }
    private bool TryHandleJumpInput()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            animator.SetBool("Jump", true);
            PlayerStateMachine.ChangeState(PlayerStateMachine.PlayerState.Jump);
            return true;
        }
        
        return false;
    }


    private void Roll(ERollState dir)
    {
        animator.SetInteger("RollCount", (int)dir);
        PlayerStateMachine.ChangeState(PlayerStateMachine.PlayerState.Roll);
    }

    private void TryHandleAttackInput()
    {
        PlayerStateMachine.ComboAttackInput();
        PlayerStateMachine.DashAttackInput();
    }

    private void UpdateMovementAnimation()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        //v = Mathf.Clamp01(v);


        if (Input.GetKey(KeyCode.LeftShift) && v > 0f)
        {
            v = Mathf.Lerp(0f, 2f, v); // 결과: 0 ~ 2
            moveSpeed = 4.0f;
        }
        else
        {
            moveSpeed = 2.0f;
        }

        // 애니메이션 파라미터 설정
        animator.SetFloat("MoveLeftRight", h);
        animator.SetFloat("MoveForWard", v);

    }

    public override void OnAttackAnimationEnd()
    {

    }
}