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

    public RollState(PlayerStateMachine.PlayerState key, Animator animator, PlayerStateMachine playerStateMachine) : base(key, animator)
    {
        this.playerStateMachine = playerStateMachine;
    }

    public override void EnterState()
    {
        animator.SetTrigger("RollTrigger");      
    }
    public override void ExitState()
    {
        playerStateMachine.startRoll();
    }
    public override void UpdateState()
    {
        
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
    public override void LateUpdateState(){ }

    public override void OnAttackAnimationEnd()
    {

    }

    int hashRollCount = Animator.StringToHash("RollCount");

    public int RollCount
    {
        get => animator.GetInteger(hashRollCount);
    }

    [SerializeField] float rollSpeed = 10f;

    PlayerStateMachine playerStateMachine;
}
