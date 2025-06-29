
using Fusion;
using UnityEngine;
using static PlayerStateMachine;


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
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Jump, false);

        this.playerStateMachine = playerStateMachine;
        groundMask = playerStateMachine.groundMask;
        groundCheck = playerStateMachine.groundCheck;

    }

    private float gravity = 9.81f;


    public override void EnterState()
    {
        if (!playerStateMachine.Object.HasStateAuthority)
            return;

        float currentStamina = playerStateMachine.Stamina.currentStamina;
        float jumpCcost = playerStateMachine.JumpStaminaCost;

        playerStateMachine.Stamina.ConsumeStaminaOnServer(jumpCcost);
        playerStateMachine.NetAnim.Animator.SetBool("Jump", true);
        playerStateMachine.playerController.Move(default,gravity);

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



    public override void OnTriggerEnter(Collider collider)
    {
        // 1) 호스트에서만 충돌 처리
        if (!playerStateMachine.Object.HasStateAuthority)
            return;

        // 2) Weapon 네트워크 오브젝트 가져오기
        var weaponNetObj = collider.GetComponent<NetworkObject>();
        if (weaponNetObj == null || !collider.CompareTag("Weapon"))
            return;


        // 3) Weapon의 입력 권한자가 이 플레이어와 같다면 스킵
        if (weaponNetObj.InputAuthority == playerStateMachine.Object.InputAuthority)
            return;

        // 4) 진짜 타격 처리
        Debug.Log("충돌 감지!2");


        playerStateMachine.health.TakeDamage(playerStateMachine.AttackCost);

        playerStateMachine.BroadcastIdleEvent(PlayerState.Hit);
    }
    public override void OnTriggerExit(Collider collider) { }
    public override void OnTriggerStay(Collider collider) { }

 
    public override void OnAnimationEvent()
    {

    }
}

