using Fusion;
using System;
using UnityEngine;
using UnityEngine.UIElements;



public class WeaponNetworkObject : NetworkBehaviour
{
    // ── 네트워크로 복제될 값들 ──
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
            // InputAuthority가 ownerRef인 플레이어만 처리
            if (psm != null && psm.Object.InputAuthority == ownerRef)
            {
                // 1) 붙일 소켓(오른손/왼손) 선택
                Transform socket = (Side == HandSide.Right)
                    ? psm.RightHandTransform
                    : psm.LeftHandTransform;

                // 2) 플레이어의 CharacterController 콜라이더
                var charController = psm.GetComponent<CharacterController>();
                // 3) 무기 자식에 달린 MeshCollider
                var weaponMeshCollider = GetComponentInChildren<MeshCollider>();

                // 4) 두 콜라이더 간 충돌 무시
                if (charController != null && weaponMeshCollider != null)
                {
                    Physics.IgnoreCollision(charController, weaponMeshCollider, true);
                }

                // 5) 손 뼈대에 부모로 붙이기
                transform.SetParent(socket, worldPositionStays: false);
                return;
            }
        }
    }
}