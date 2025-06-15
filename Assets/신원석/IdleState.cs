using UnityEngine;
using static Unity.Collections.Unicode;

public class IdleState : BaseState<PlayerStateMachine.PlayerState>
{
    PlayerStateMachine playerStateMachine;
    public IdleState(PlayerStateMachine.PlayerState key,Animator animator, PlayerStateMachine PlayerStateMachine) : base(key,animator)
    {
        this.playerStateMachine = PlayerStateMachine;
    }

    public override void EnterState()
    {
        
    }
    public override void ExitState() => Debug.Log("Exit Idle");
    public override void UpdateState()
    {

    }

    private float attack = 0f;  // 필드로 선언


    public override void FixedUpdateState()
    {

    
    }

    public override PlayerStateMachine.PlayerState GetNextState()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            return PlayerStateMachine.PlayerState.Move;    
        if(Input.GetKeyDown(KeyCode.L))
            return PlayerStateMachine.PlayerState.Switch;
        if (Input.GetMouseButtonDown(0) && playerStateMachine.isWeapon ==true && playerStateMachine.AnimHandler.WeaponCount != (int)ItemState.Bow)
            return PlayerStateMachine.PlayerState.Attack;
        if (Input.GetKey(KeyCode.LeftControl))
            return PlayerStateMachine.PlayerState.Jump;
        if (Input.GetMouseButtonDown(1) && playerStateMachine.isWeapon == true && playerStateMachine.AnimHandler.WeaponCount == (int)ItemState.Bow)
            return PlayerStateMachine.PlayerState.BowAttack;



        return PlayerStateMachine.PlayerState.Idle;
    }

   

    public override void OnTriggerEnter(Collider collider) { }
    public override void OnTriggerExit(Collider collider) { }
    public override void OnTriggerStay(Collider collider) { }

    public override void LateUpdateState(){ }

    public override void OnAttackAnimationEnd()
    {

    }
}