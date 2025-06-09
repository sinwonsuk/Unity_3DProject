using UnityEngine;

public class IdleState : BaseState<PlayerStateMachine.PlayerState>
{
    PlayerStateMachine PlayerStateMachine;

    public IdleState(PlayerStateMachine.PlayerState key,Animator animator, PlayerStateMachine PlayerStateMachine) : base(key,animator)
    {
        this.PlayerStateMachine = PlayerStateMachine;
    }

    public override void EnterState()
    {
        
    }
    public override void ExitState() => Debug.Log("Exit Idle");
    public override void UpdateState()
    {
        Debug.Log("adad Idle");


       
    }




    public override void FixedUpdateState()
    {

    }

    public override PlayerStateMachine.PlayerState GetNextState()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            return PlayerStateMachine.PlayerState.Move;    
        if(Input.GetKey(KeyCode.L))
            return PlayerStateMachine.PlayerState.Switch;
        if (Input.GetMouseButtonDown(0) && PlayerStateMachine.isWeapon ==true)
            return PlayerStateMachine.PlayerState.Attack;

        if (Input.GetKey(KeyCode.LeftControl))
            return PlayerStateMachine.PlayerState.Jump;

        return PlayerStateMachine.PlayerState.Idle;
    }

   

    public override void OnTriggerEnter(Collider collider) { }
    public override void OnTriggerExit(Collider collider) { }
    public override void OnTriggerStay(Collider collider) { }

    public override void LateUpdateState(){ }
}