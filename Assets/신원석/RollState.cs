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
        playerStateMachine.isRoll = false;


        playerStateMachine.NetAnim.Animator.SetTrigger("RollTrigger");

        rollDirection = GetDirectionFromCount(playerStateMachine.AnimHandler.RollCount);

    }
    public override void ExitState()
    {
        //playerStateMachine.startRoll();
        playerStateMachine.NetAnim.Animator.ResetTrigger("RollTrigger");
        playerStateMachine.BroadcastIdleEvent(PlayerStateMachine.PlayerState.Idle);

    }
    public override void FixedUpdateState()
    {
        if (!playerStateMachine.Object.HasStateAuthority)
            return;
        if (playerStateMachine.isRoll == true)
            return;

           playerStateMachine.playerController.Move(rollDirection * rollSpeed );
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
    public override void OnAnimationEvent()
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
