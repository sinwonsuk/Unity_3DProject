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
    }
    public override void ExitState()
    {
        playerStateMachine.startRoll();
    }
    public override void FixedUpdateState()
    {    
        playerStateMachine.MoveRoll(RollCount);
    }

    public override PlayerStateMachine.PlayerState GetNextState()
    {
        return PlayerStateMachine.PlayerState.Roll;
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

    [SerializeField] float rollSpeed = 10f;

    PlayerStateMachine playerStateMachine;
}
