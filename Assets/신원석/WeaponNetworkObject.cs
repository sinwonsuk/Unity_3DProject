using Fusion;
using System;
using UnityEngine;
using UnityEngine.UIElements;



public class WeaponNetworkObject : NetworkBehaviour
{
    // ���� ��Ʈ��ũ�� ������ ���� ����
    [Networked] public HandSide Side { get; set; }
    [Networked] public HandSide ArrowSide { get; set; }

    const string PlayerTag = "Player";

    public Action action;

    public WeaponInfoConfig weaponInfoConfig;

    [Networked] public ItemClass ItemClass { get; set; } = ItemClass.One;

    public override void Spawned()
    {
        gameObject.SetActive(false);
      
        switch (ItemClass)
        {
            case ItemClass.None:
                transform.localScale = Vector3.zero;
                break;
            case ItemClass.One:
                transform.localScale = weaponInfoConfig.ScaleOne;
                break;
            case ItemClass.Two:
                transform.localScale = weaponInfoConfig.ScaleTwo;
                break;
            case ItemClass.Three:
                transform.localScale = weaponInfoConfig.ScaleThree;
                break;
            default:
                break;
        }

        AttachToOwner(Object.InputAuthority);
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
    }
}