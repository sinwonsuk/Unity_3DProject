using UnityEngine;
using Fusion;
using System;



public class WeaponNetworkObject : NetworkBehaviour
{
    // ���� ��Ʈ��ũ�� ������ ���� ����
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
            // �� �÷��̾ weapon.InputAuthority�� ���ٸ�
            if (psm != null && psm.Object.InputAuthority == ownerRef)
            {
                Transform socket = (weapon.Side == HandSide.Right) ? psm.RightHandTransform: psm.LeftHandTransform;

                transform.SetParent(socket, worldPositionStays: false);
                return;
            }
        }

      
    }
}