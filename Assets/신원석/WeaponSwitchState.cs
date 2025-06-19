using UnityEngine;
using UnityEngine.AI;

public class WeaponSwitchState : BaseState<PlayerStateMachine.PlayerState>
{
    PlayerStateMachine playerStateMachine;
    public WeaponSwitchState(PlayerStateMachine.PlayerState key, Animator animator,PlayerStateMachine playerStateMachine) : base(key, animator)
    {
        this.playerStateMachine = playerStateMachine;
    }

    public override void EnterState()
    {



        playerStateMachine.WeaponManager.Equip(ItemState.Magic,isDir.Left);
        playerStateMachine.AnimHandler.ChangeWeapon(ItemState.Magic);
        playerStateMachine.isWeapon = true;
    }
    public override void ExitState()
    {

    }
    public override void UpdateState()
    {
       
    }

    public override PlayerStateMachine.PlayerState GetNextState()
    {      
        return PlayerStateMachine.PlayerState.Idle;
    }

    public override void OnTriggerEnter(Collider collider) { }
    public override void OnTriggerExit(Collider collider) { }
    public override void OnTriggerStay(Collider collider) { }

    public override void FixedUpdateState(){ }

    public override void LateUpdateState(){ }

    public override void OnAttackAnimationEnd()
    {

    }


    [SerializeField] float rotationSpeed = 500f;

}
