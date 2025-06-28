using Fusion;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class HitState : BaseState<PlayerStateMachine.PlayerState>
{
    PlayerStateMachine playerStateMachine;

    private HashSet<PlayerRef> _hitSet = new HashSet<PlayerRef>();
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
        //if (!playerStateMachine.Object.HasStateAuthority)
        //    return;

        //// 1) �˻� ��ġ: weaponTip�� �ƴ� ĳ���� �߽� �Ǵ� ���ϴ� ����Ʈ
        //Vector3 center = playerStateMachine.HitTransform.position;
        //// 2) �ݰ�: ���� ����/�β��� ĳ���� �ݰ濡 ���� ����
        //float radius = 0.7f;
        //// 3) ���⸸ �ɸ����� ���̾� ����ũ
        //int layerMask = LayerMask.GetMask("Weapon");

        //// 4) OverlapSphere�� �� ���� ��� ���� Collider�� �����´�
        //Collider[] hits = Physics.OverlapSphere(center, radius, layerMask, QueryTriggerInteraction.Collide);

        //foreach (var col in hits)
        //{
        //    var weaponObj = col.GetComponent<NetworkObject>();
        //    if (weaponObj == null)
        //        continue;

        //    var attacker = weaponObj.InputAuthority;
        //    if (attacker == playerStateMachine.Object.InputAuthority)
        //        continue;

        //    // ������ ó��
        //    playerStateMachine.health.TakeDamage(1);
        //    playerStateMachine.HitCount = Mathf.Min(playerStateMachine.HitCount + 1, 4);
        //    playerStateMachine.PlayHitAnimation(playerStateMachine.HitCount);
        //}

        playerStateMachine.action.Invoke();
    }

    public override PlayerStateMachine.PlayerState GetNextState()
    {
        return PlayerStateMachine.PlayerState.Hit;
    }

    public override void OnTriggerEnter(Collider collider)
    {
        if (!playerStateMachine.Object.HasStateAuthority)
            return;


        var weaponNetObj = collider.GetComponent<NetworkObject>();
        if (weaponNetObj == null || !collider.CompareTag("Weapon"))
            return;


        if (weaponNetObj.InputAuthority == playerStateMachine.Object.InputAuthority)
            return;

        playerStateMachine.AnimHandler.SetHitTrigger();
        playerStateMachine.AnimHandler.SetHitBool(true);
        playerStateMachine.health.RequestDamage(20);
    }
    public override void OnTriggerExit(Collider collider) { }
    public override void OnTriggerStay(Collider collider) { }
    public override void OnAnimationEvent()
    {
        playerStateMachine.hitMap.Clear();
    }


}
