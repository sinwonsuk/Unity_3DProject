using UnityEngine;
using UnityEngine.AI;
using static Unity.Collections.Unicode;
using static UnityEngine.Rendering.DebugUI;
using Fusion;
public class MoveState : BaseState<PlayerStateMachine.PlayerState>
{
    PlayerStateMachine playerStateMachine;

    [SerializeField] float rotationSpeed = 500f;
    float moveSpeed = 2.0f;

    public MoveState(PlayerStateMachine.PlayerState key, Animator animator, PlayerStateMachine playerStateMachine)
        : base(key, animator)
    {
        this.playerStateMachine = playerStateMachine;
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
        playerStateMachine.MoveInput();


        if (TryHandleRollInput()) 
            return;

        TryHandleAttackInput();

       
    }
    public override void LateUpdateState()
    {
        //playerStateMachine.testte();
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
        return playerStateMachine.RollInput();
    }
    private bool TryHandleJumpInput()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            animator.SetBool("Jump", true);
            playerStateMachine.ChangeState(PlayerStateMachine.PlayerState.Jump);
            return true;
        }
        
        return false;
    }

    private void TryHandleAttackInput()
    {
        playerStateMachine.ComboAttackInput();
        playerStateMachine.DashAttackInput();

        if(playerStateMachine.inputHandler.IsRightAttackPressed() && playerStateMachine.isWeapon ==true && playerStateMachine.AnimHandler.WeaponCount == (int)ItemState.Bow)
        {
            playerStateMachine.ChangeState(PlayerStateMachine.PlayerState.BowAttack);
            return;
        }
        if (playerStateMachine.inputHandler.IsRightAttackPressed() && playerStateMachine.isWeapon == true && playerStateMachine.AnimHandler.WeaponCount == (int)ItemState.Magic)
        {
            playerStateMachine.ChangeState(PlayerStateMachine.PlayerState.Magic);
            return;
        }

    }

    private void UpdateMovementAnimation()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

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