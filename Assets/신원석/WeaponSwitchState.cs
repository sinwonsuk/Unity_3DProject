using Fusion;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static PlayerStateMachine;

public class WeaponSwitchState : BaseState<PlayerStateMachine.PlayerState>
{
    PlayerStateMachine playerStateMachine;
    public WeaponSwitchState(PlayerStateMachine.PlayerState key,PlayerStateMachine playerStateMachine) : base(key)
    {
        this.playerStateMachine = playerStateMachine;
    }

    public override void EnterState()
    {
        if (playerStateMachine.Object.HasStateAuthority)
        {
            ItemClass itemClass = playerStateMachine.ItemClass;
            ItemState itemState = playerStateMachine.itemState;



            playerStateMachine.SetWeapon(true);
            playerStateMachine.WeaponManager.RequestEquip(itemState, HandSide.Right, itemClass, playerStateMachine.owner);
            playerStateMachine.WeaponManager.ChangeWeaponClass();


            playerStateMachine.AnimHandler.ChangeWeapon(itemState);


            playerStateMachine.AttackStaminaCost = playerStateMachine.WeaponManager.currentWeapon.GetComponent<WeaponNetworkObject>().weaponInfoConfig.Stamina;
            playerStateMachine.AttackCost = playerStateMachine.WeaponManager.currentWeapon.GetComponent<WeaponNetworkObject>().weaponInfoConfig.Attack;

        }

        playerStateMachine.AnimHandler.ChangeWeapon(playerStateMachine.itemState);
    }
    public override void ExitState()
    {




    }

    public override PlayerStateMachine.PlayerState GetNextState()
    {      
        return PlayerStateMachine.PlayerState.Switch;
    }

    public override void OnTriggerEnter(Collider collider) { }
    public override void OnTriggerExit(Collider collider) { }
    public override void OnTriggerStay(Collider collider) { }

    public override void FixedUpdateState(){ }

    public override void OnAnimationEvent()
    {

    }
}
