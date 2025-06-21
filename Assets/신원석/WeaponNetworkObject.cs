using UnityEngine;
using Fusion;
using System;



public class WeaponNetworkObject : NetworkBehaviour
{
    // ── 네트워크로 복제될 값들 ──
    [Networked] public HandSide Side { get; set; }
    [Networked] public HandSide ArrowSide { get; set; }

    const string PlayerTag = "Player";

    public Action action;

    public Transform Hand { get; set; }

    public override void Spawned()
    {
        AttachToOwner(Object.InputAuthority);
    }

    public void AttachToOwner(PlayerRef ownerRef)
    {

        var players = GameObject.FindGameObjectsWithTag(PlayerTag);
        foreach (var go in players)
        {
            var psm = go.GetComponent<PlayerStateMachine>();
            var weapon = go.GetComponent<WeaponManager>();
            // 이 플레이어가 weapon.InputAuthority와 같다면
            if (psm != null && psm.Object.InputAuthority == ownerRef)
            {
                Transform socket = (weapon.Side == HandSide.Right) ? psm.RightHandTransform: psm.LeftHandTransform;

                transform.SetParent(socket, worldPositionStays: false);
                return;
            }
        }

      
    }
}