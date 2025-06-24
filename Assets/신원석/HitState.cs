using UnityEngine;
using Fusion;
using static UnityEngine.UI.GridLayoutGroup;

public class HitState : BaseState<PlayerStateMachine.PlayerState>
{
    PlayerStateMachine playerStateMachine;


    public HitState(PlayerStateMachine.PlayerState key, PlayerStateMachine stateMachine) : base(key)
    {
        this.playerStateMachine = stateMachine;
    }

    public override void EnterState()
    {

        playerStateMachine.Combat.StartHit();
        playerStateMachine.hitSet.Clear();  // �������� �ʱ�ȭ

    }
    public override void ExitState()
    {

            playerStateMachine.SetIsAttackFalse();
            playerStateMachine.AnimHandler.SetHitBool(false);
        
    }

    public override void FixedUpdateState()
    {
        
    }

    public override PlayerStateMachine.PlayerState GetNextState()
    {
        return PlayerStateMachine.PlayerState.Hit;
    }

    public override void OnTriggerEnter(Collider collider)
    {
        if (!playerStateMachine.Object.HasStateAuthority)
        {
            return;
        }


        // 2) Weapon ��Ʈ��ũ ������Ʈ ��������
        var weaponNetObj = collider.GetComponent<NetworkObject>();
            if (weaponNetObj == null || !collider.CompareTag("Weapon"))
                return;

            // 3) Weapon�� �Է� �����ڰ� �� �÷��̾�� ���ٸ� ��ŵ
            if (weaponNetObj.InputAuthority == playerStateMachine.Object.InputAuthority)
                return;
        


        playerStateMachine.HitCount += 1;

        if (playerStateMachine.HitCount > 4)
        {
            playerStateMachine.HitCount = 4;
        }

        playerStateMachine.PlayHitAnimation(playerStateMachine.HitCount);

    }
    public override void OnTriggerExit(Collider collider) { }
    public override void OnTriggerStay(Collider collider) { }
    public override void OnHitAnimationEvent()
    {
        playerStateMachine.Combat.OnAttackAnimationEnd();
    }


}
