using UnityEngine;
using UnityEngine.AI;
using Fusion;
using static PlayerStateMachine;

public class MoveState : BaseState<PlayerStateMachine.PlayerState>
{
    PlayerStateMachine playerStateMachine;

    [SerializeField] float rotationSpeed = 500f;
    float moveSpeed = 2.0f;

    public MoveState(PlayerStateMachine.PlayerState key, PlayerStateMachine playerStateMachine)
        : base(key)
    {
        this.playerStateMachine = playerStateMachine;
    }

    public override void EnterState()
    {

    }

    public override void ExitState()
    {
 
        playerStateMachine.NetAnim.Animator.SetFloat("MoveLeftRight", 0);
        playerStateMachine.NetAnim.Animator.SetFloat("MoveForWard", 0);

        moveSpeed = 2.0f;
    }
    public void Move()
    {
        playerStateMachine.MoveInput();
    }

    public override void FixedUpdateState()
    {
        TryHandleRollInput();
        TryHandleJumpInput();
        TryHandleAttackInput();
        UpdateMovementAnimation();
        Move();
        ChangeIdle();
    }
    public override PlayerStateMachine.PlayerState GetNextState()
    {
        return PlayerStateMachine.PlayerState.Move;
    }
    public void ChangeIdle()
    {
        if (playerStateMachine.inputHandler.IsMove() == false)
        {
            if (playerStateMachine.Object.HasInputAuthority)
            {
                playerStateMachine.RPC_BroadcastState(PlayerState.Idle);
                return;
            }
        }
    }
    public override void OnTriggerEnter(Collider collider) { }
    public override void OnTriggerExit(Collider collider) { }
    public override void OnTriggerStay(Collider collider) { }

    private void TryHandleRollInput()
    {
         if (playerStateMachine.IsWeapon == false)
            return;

        if (playerStateMachine.inputHandler.RollInput(out ERollState dir))
            Roll(dir);
    }
    private void Roll(ERollState dir)
    {
        playerStateMachine.NetAnim.Animator.SetInteger("RollCount", (int)dir);
        
        if (playerStateMachine.Object.HasStateAuthority)
        {
            playerStateMachine.SyncedState = PlayerState.Roll;
            playerStateMachine.RollCount = (int)dir;
        }         
        else
        {
            playerStateMachine.RPC_BroadcastState(PlayerState.Roll);
            playerStateMachine.RPC_SetRollCount((int)dir);
        }
          
        return;
    }

    private bool TryHandleJumpInput()
    {
        if (playerStateMachine.inputHandler.IsCtrlButtonPress())
        {
            playerStateMachine.NetAnim.Animator.SetBool("Jump", true);

            playerStateMachine.playerController.Move(playerStateMachine.playerController.transform.position, 5);

            playerStateMachine.RPC_BroadcastState(PlayerState.Jump);



            return true;
        }
        
        return false;
    }

    private void TryHandleAttackInput()
    {
        ComboAttackInput();

        if (playerStateMachine.cameraManager.isCameraCheck == false)
            return;

        if (playerStateMachine.inputHandler.IsRightAttackPressed() && playerStateMachine.IsWeapon ==true && playerStateMachine.AnimHandler.WeaponCount == (int)ItemState.Bow)
        {
            playerStateMachine.RPC_BroadcastState(PlayerStateMachine.PlayerState.BowAttack);
            return;
        }

        if (playerStateMachine.inputHandler.IsRightAttackPressed() && playerStateMachine.IsWeapon == true && playerStateMachine.AnimHandler.WeaponCount == 4)
        {
            playerStateMachine.RPC_BroadcastState(PlayerStateMachine.PlayerState.Magic);
            return;
        }

    }
    public void ComboAttackInput()
    {
        if (playerStateMachine.IsWeapon == false)
            return;

        if (playerStateMachine.inputHandler.IsAttackPressed() && playerStateMachine.Object.HasInputAuthority && playerStateMachine.AnimHandler.WeaponCount != (int)ItemState.Bow && playerStateMachine.AnimHandler.WeaponCount != 4)
        {
            if (playerStateMachine.Object.HasStateAuthority)
            {
                playerStateMachine.SyncedState = PlayerState.Attack;
            }
            else
            {
                playerStateMachine.RPC_BroadcastState(PlayerState.Attack);
            }
        }
    }
    private void UpdateMovementAnimation()
    {
        //if (playerStateMachine.IsProxy == true || playerStateMachine.Runner.IsForward == false)
        //    return;

        if (playerStateMachine.GetInput(out NetworkInputData data))
        {
            float x = data.moveAxis.x;
            float z = data.moveAxis.z;

            if (Input.GetKey(KeyCode.LeftShift) && z > 0f)
            {
                z = Mathf.Lerp(0f, 2f, z); // °á°ú: 0 ~ 2
                moveSpeed = 4.0f;
            }
            else
            {
                moveSpeed = 2.0f;   
            }         
            playerStateMachine.NetAnim.Animator.SetFloat("MoveLeftRight", x);
            playerStateMachine.NetAnim.Animator.SetFloat("MoveForWard", z);
        }
    }

    public override void OnHitAnimationEvent()
    {

    }
}