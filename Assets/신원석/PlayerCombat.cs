using Fusion;
using System;
using System.Collections;
using UnityEngine;

public class PlayerCombat 
{
    private PlayerStateMachine player;
    private bool nextComboQueued = false;



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

    public void StartAttack()
    {
        AttackCount = 1;
        nextComboQueued = false;
        player.AnimHandler.SetAttackCount(AttackCount);
        player.AnimHandler.SetAttackBool(true);
    }




    public void TryQueueNextCombo()
    {
        if (AttackCount < 4)
            nextComboQueued = true;
    }

    public void OnAnimationEnd()
    {


        if (nextComboQueued && AttackCount < 4)
        {
            AttackCount++;
            nextComboQueued = false;
            player.SetIsAttackTrue();
            //player.AnimHandler.SetAttackCount(AttackCount);
            //player.AnimHandler.SetAttackBool(true);
        }
        else
        {   
            player.SetIsAttackFalse();
        }
      
    }


}