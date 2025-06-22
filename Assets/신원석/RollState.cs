using Fusion;
using UnityEngine;
using UnityEngine.AI;
using static Unity.Collections.Unicode;

public enum ERollState
{
    Left,
    Forward,
    Backward,
    Right,
}

public class RollState : BaseState<PlayerStateMachine.PlayerState>
{

    public RollState(PlayerStateMachine.PlayerState key, Animator animator, PlayerStateMachine playerStateMachine) : base(key)
    {
        this.playerStateMachine = playerStateMachine;
    }

    public override void EnterState()
    {
        playerStateMachine.NetAnim.Animator.SetTrigger("RollTrigger");

        rollDirection = GetDirectionFromCount(playerStateMachine.AnimHandler.RollCount);

    }
    public override void ExitState()
    {
        playerStateMachine.startRoll();
        playerStateMachine.NetAnim.Animator.ResetTrigger("RollTrigger");
    }
    public override void FixedUpdateState()
    {
        if (playerStateMachine.isRoll == true)
            return;

        //if (!playerStateMachine.Object.HasStateAuthority && playerStateMachine.Object.HasInputAuthority)
        //    playerStateMachine.playerController.Move(rollDirection * rollSpeed * playerStateMachine.Runner.DeltaTime);

        //else if (playerStateMachine.Object.HasStateAuthority)
        //    playerStateMachine.playerController.Move(rollDirection * rollSpeed * playerStateMachine.Runner.DeltaTime);


        playerStateMachine.AnimHandler.ChangeRoll(playerStateMachine.AnimHandler.RollCount);

    }

    public override PlayerStateMachine.PlayerState GetNextState()
    {
        return PlayerStateMachine.PlayerState.Roll;
    }

    private Vector3 GetDirectionFromCount(int count)
    {
        switch ((ERollState)count)
        {
            case ERollState.Forward: return playerStateMachine.transform.forward;
            case ERollState.Backward: return -playerStateMachine.transform.forward;
            case ERollState.Left: return -playerStateMachine.transform.right;
            case ERollState.Right: return playerStateMachine.transform.right;
            default: return Vector3.zero;
        }
    }

    public override void OnTriggerEnter(Collider collider) { }
    public override void OnTriggerExit(Collider collider) { }
    public override void OnTriggerStay(Collider collider) { }
    public override void OnAttackAnimationEnd()
    {

    }

    int hashRollCount = Animator.StringToHash("RollCount");

    public int RollCount
    {
        get => playerStateMachine.NetAnim.Animator.GetInteger(hashRollCount);
    }
    Vector3 rollDirection;

    [SerializeField] float rollSpeed = 10f;

    PlayerStateMachine playerStateMachine;
}
