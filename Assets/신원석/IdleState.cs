using UnityEngine;

public class IdleState : BaseState<PlayerStateMachine.PlayerState>
{
    public IdleState(PlayerStateMachine.PlayerState key,Animator animator) : base(key,animator)
    { 

    }

    public override void EnterState()
    {
        
    }
    public override void ExitState() => Debug.Log("Exit Idle");
    public override void UpdateState() => Debug.Log("Idle Updating...");
    public override void FixedUpdateState() { }

    public override PlayerStateMachine.PlayerState GetNextState()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            return PlayerStateMachine.PlayerState.Move;
        if(Input.GetKey(KeyCode.L))
            return PlayerStateMachine.PlayerState.Switch;
        if (Input.GetMouseButtonDown(0))
            return PlayerStateMachine.PlayerState.Attack;


        return PlayerStateMachine.PlayerState.Idle;
    }

   

    public override void OnTriggerEnter(Collider collider) { }
    public override void OnTriggerExit(Collider collider) { }
    public override void OnTriggerStay(Collider collider) { }

}