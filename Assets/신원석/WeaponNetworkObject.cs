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


        var weaponCols = GetComponentsInChildren<Collider>();

    }

    public void AttachToOwner(PlayerRef ownerRef)
    {

        var players = GameObject.FindGameObjectsWithTag(PlayerTag);
        foreach (var go in players)
        {
            var psm = go.GetComponent<PlayerStateMachine>();
            // InputAuthority�� ownerRef�� �÷��̾ ó��
            if (psm != null && psm.Object.InputAuthority == ownerRef)
            {
                // 1) ���� ����(������/�޼�) ����
                Transform socket = (Side == HandSide.Right)
                    ? psm.RightHandTransform
                    : psm.LeftHandTransform;

                // 2) �÷��̾��� CharacterController �ݶ��̴�
                var charController = psm.GetComponent<CharacterController>();
                // 3) ���� �ڽĿ� �޸� MeshCollider
                var weaponMeshCollider = GetComponentInChildren<MeshCollider>();

                // 4) �� �ݶ��̴� �� �浹 ����
                if (charController != null && weaponMeshCollider != null)
                {
                    Physics.IgnoreCollision(charController, weaponMeshCollider, true);
                }

                // 5) �� ���뿡 �θ�� ���̱�
                transform.SetParent(socket, worldPositionStays: false);
                return;
            }
        }

        // 1) ������ �ڱ� �ڽ�(Weapon) �ݶ��̴�



    }
}