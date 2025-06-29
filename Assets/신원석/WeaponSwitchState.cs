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

        if (!playerStateMachine.Object.HasInputAuthority)
            return;
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.ChangeWeapon, false);

        playerStateMachine.AnimHandler.ChangeWeapon(playerStateMachine.itemState);
    }
    public override void ExitState()
    {




    }

    public override PlayerStateMachine.PlayerState GetNextState()
    {      
        return PlayerStateMachine.PlayerState.Switch;
    }

    public override void OnTriggerEnter(Collider collider)
    {
        // 1) ȣ��Ʈ������ �浹 ó��
        if (!playerStateMachine.Object.HasStateAuthority)
            return;

        if (playerStateMachine.isDeath == true)
            return;
        // 2) Weapon ��Ʈ��ũ ������Ʈ ��������
        var weaponNetObj = collider.GetComponent<NetworkObject>();
        if (weaponNetObj == null || !collider.CompareTag("Weapon"))
            return;


        // 3) Weapon�� �Է� �����ڰ� �� �÷��̾�� ���ٸ� ��ŵ
        if (weaponNetObj.InputAuthority == playerStateMachine.Object.InputAuthority)
            return;



        int attack = weaponNetObj.gameObject.GetComponent<WeaponNetworkObject>().weaponInfoConfig.Attack;

        playerStateMachine.health.RequestDamage(attack);

        weaponNetObj.GetComponent<WeaponNetworkObject>().GetComponent<MeshCollider>().enabled = false;

        playerStateMachine.BroadcastIdleEvent(PlayerState.Hit);
    }
    public override void OnTriggerExit(Collider collider) { }
    public override void OnTriggerStay(Collider collider) { }

    public override void FixedUpdateState(){ }

    public override void OnAnimationEvent()
    {

    }
}
