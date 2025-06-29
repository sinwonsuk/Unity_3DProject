using Fusion;
using System;
using System.Collections;
using UnityEngine;

public class PlayerCombat 
{
    private PlayerStateMachine player;
    private bool nextComboQueued = false;
    private bool nextHitQueued = false;
    public float stamina { get; set; }
    public bool hasConsumedStamina { get; set; } = false;
    public Action<int,RpcInfo> Rpc_AttackAction;
    public Action<RpcInfo> Rpc_EndAttack;

    public PlayerCombat(PlayerStateMachine player)
    {
        this.player = player;
    }

    public int AttackCount
    {
        get => player.AttackCount;      // 네트워크 프로퍼티 읽기
        set => player.AttackCount = value;  // 네트워크 프로퍼티 쓰기
    }

    public int HitCount
    {
        get => player.HitCount;      // 네트워크 프로퍼티 읽기
        set => player.HitCount = value;  // 네트워크 프로퍼티 쓰기
    }


    public void StartAttack()
    {
        stamina = player.WeaponManager.currentWeapon.GetComponent<WeaponNetworkObject>().weaponInfoConfig.Stamina;
        player.Stamina.ConsumeStaminaOnServer(1);
        AttackCount = 1;
        nextHitQueued = false;
        player.AnimHandler.SetAttackCount(AttackCount);
        player.AnimHandler.SetAttackBool(true);
        hasConsumedStamina = false;
    }
    public void StartHit()
    {
        HitCount = 1;
        nextHitQueued = false;
        player.AnimHandler.SetHitTrigger();
        player.AnimHandler.SetHitCount(HitCount);
        player.AnimHandler.SetHitBool(true);
    }

    public void TryHitNextCombo()
    {
        if (HitCount < 4)
            nextHitQueued = true;
    }

    public void TryQueueNextCombo()
    {
        if (AttackCount < 4)
            nextComboQueued = true;
    }

    public void OnAttackAnimationEnd()
    {


        if (nextComboQueued && AttackCount < 4 )
        {
            player.Stamina.ConsumeStaminaOnServer(1);

            AttackCount++;
            nextComboQueued = false;
            player.SetIsAttackTrue();
            player.ClearHitSet();       
            
        }
        else
        {   
            player.SetIsAttackFalse();
            player.ClearHitSet();
        }
      
    }
    public void OnHitAnimationEnd()
    {
        if (nextHitQueued && HitCount < 4)
        {
            
            HitCount++;
            nextHitQueued = false;
            player.SetIsHitTrue();
            player.ClearHitSet();
        }
    }


}