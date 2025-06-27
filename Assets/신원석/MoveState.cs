using Fusion;
using UnityEngine;
using UnityEngine.AI;
using static PlayerStateMachine;
using static Unity.Collections.Unicode;

public class MoveState : BaseState<PlayerStateMachine.PlayerState>
{
    PlayerStateMachine playerStateMachine;

    [SerializeField] float rotationSpeed = 500f;


    float moveX = 0.0f;
    float moveZ = 0.0f;

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
        //playerStateMachine.NetAnim.Animator.SetFloat("MoveLeftRight", 0);
        //playerStateMachine.NetAnim.Animator.SetFloat("MoveForWard", 0);

        //playerStateMachine.moveX = 0.0f;
        //playerStateMachine.moveZ = 0.0f;

        playerStateMachine.MoveSpeed = 5.0f;
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
                playerStateMachine.BroadcastIdleEvent(PlayerState.Idle);
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
            playerStateMachine.BroadcastIdleEvent(PlayerState.Roll);
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

            playerStateMachine.BroadcastIdleEvent(PlayerState.Jump);

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
            playerStateMachine.BroadcastIdleEvent(PlayerStateMachine.PlayerState.BowAttack);
            return;
        }

        if (playerStateMachine.inputHandler.IsRightAttackPressed() && playerStateMachine.IsWeapon == true && playerStateMachine.AnimHandler.WeaponCount == 4)
        {
            playerStateMachine.BroadcastIdleEvent(PlayerStateMachine.PlayerState.Magic);
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
                playerStateMachine.BroadcastIdleEvent(PlayerState.Attack);
            }
        }
    }
    private void UpdateMovementAnimation()
    {

        if (playerStateMachine.GetInput(out NetworkInputData data))
        {

            float targetX = data.moveAxis.x;
            float targetZ = data.moveAxis.z;

            playerStateMachine.moveZ = data.moveAxis.z;

            // 2) ���� smoothX �� targetX�� õõ�� �̵�
            playerStateMachine.moveX = Mathf.MoveTowards(
                playerStateMachine.moveX,
                targetX,
                playerStateMachine.Runner.DeltaTime * 5.0f
            );



            Mathf.MoveTowards(playerStateMachine.moveZ, 0f, playerStateMachine.Runner.DeltaTime * 5.0f);

            if (playerStateMachine.inputHandler.IsShiftButtonPress() && playerStateMachine.moveZ > 0f /*&& playerStateMachine.Stamina.currentStamina >= 0.0f*/)
            {
                playerStateMachine.moveZ = Mathf.Lerp(0f, 2f, playerStateMachine.moveZ); // ���: 0 ~ 2
                playerStateMachine.MoveSpeed = 10.0f;
                //playerStateMachine.Stamina.UseStamina(playerStateMachine.Runner.DeltaTime * 10.0f);
                EventBus<isRunning>.Raise(new isRunning(true));

                playerStateMachine.Stamina.IsStamania = true;
            }
            else
            {
                playerStateMachine.MoveSpeed = 5.0f;
                playerStateMachine.Stamina.IsStamania = false;
            }    
            
            playerStateMachine.NetAnim.Animator.SetFloat("MoveLeftRight", playerStateMachine.moveX);
            playerStateMachine.NetAnim.Animator.SetFloat("MoveForWard", playerStateMachine.moveZ);
        }
    }

    public override void OnHitAnimationEvent()
    {

    }

   
}