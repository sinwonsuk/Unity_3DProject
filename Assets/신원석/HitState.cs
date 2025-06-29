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
        playerStateMachine.hitSet.Clear();  // 스윙마다 초기화

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

        //// 1) 검사 위치: weaponTip이 아닌 캐릭터 중심 또는 원하는 포인트
        //Vector3 center = playerStateMachine.HitTransform.position;
        //// 2) 반경: 무기 길이/두께나 캐릭터 반경에 맞춰 조절
        //float radius = 0.7f;
        //// 3) 무기만 걸리도록 레이어 마스크
        //int layerMask = LayerMask.GetMask("Weapon");

        //// 4) OverlapSphere로 한 번에 모든 무기 Collider를 가져온다
        //Collider[] hits = Physics.OverlapSphere(center, radius, layerMask, QueryTriggerInteraction.Collide);

        //foreach (var col in hits)
        //{
        //    var weaponObj = col.GetComponent<NetworkObject>();
        //    if (weaponObj == null)
        //        continue;

        //    var attacker = weaponObj.InputAuthority;
        //    if (attacker == playerStateMachine.Object.InputAuthority)
        //        continue;

        //    // 데미지 처리
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
