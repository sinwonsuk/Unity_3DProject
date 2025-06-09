using Fusion;
using UnityEngine;
using UnityEngine.AI;

public enum ERollState
{
    Left,
    Forward,
    Backward,
    Right,
}

public class RollState : BaseState<PlayerStateMachine.PlayerState>
{
    PlayerStateMachine PlayerStateMachine;
    Transform transform;

    public RollState(PlayerStateMachine.PlayerState key, Animator animator, PlayerStateMachine playerStateMachine) : base(key, animator)
    {
        this.PlayerStateMachine = playerStateMachine;

        transform = PlayerStateMachine.transform;
    }

    public override void EnterState()
    {
        animator.SetTrigger("RollTrigger");      
    }
    public override void ExitState()
    {
       
    }
    public override void UpdateState()
    {
        
    }

    public override void FixedUpdateState()
    {
        float deltaTime = PlayerStateMachine.Runner.DeltaTime;

        if ((ERollState)RollCount == ERollState.Left)
        {
            PlayerStateMachine.playerController.Move(-transform.right * rollSpeed * deltaTime);
        }
        else if ((ERollState)RollCount == ERollState.Right)
        {
            PlayerStateMachine.playerController.Move(transform.right * rollSpeed * deltaTime);
        }
        else if ((ERollState)RollCount == ERollState.Forward)
        {
            PlayerStateMachine.playerController.Move(transform.forward * rollSpeed * deltaTime);
        }
        else if ((ERollState)RollCount == ERollState.Backward)
        {
            PlayerStateMachine.playerController.Move(-transform.forward * rollSpeed * deltaTime);
        }
    }



    public override PlayerStateMachine.PlayerState GetNextState()
    {
        return PlayerStateMachine.PlayerState.Roll;
    }

    public override void OnTriggerEnter(Collider collider) { }
    public override void OnTriggerExit(Collider collider) { }
    public override void OnTriggerStay(Collider collider) { }

    public override void LateUpdateState(){ }

    float moveSpeed = 2.0f;


    int hashRollCount = Animator.StringToHash("RollCount");


    public int RollCount
    {
        get => animator.GetInteger(hashRollCount);
    }

    [SerializeField] float rollSpeed = 10f;
    Quaternion targetRotation;
}
