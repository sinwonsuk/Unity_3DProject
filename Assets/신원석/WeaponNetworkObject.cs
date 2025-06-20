using UnityEngine;
using Fusion;
using System;

public class WeaponNetworkObject : NetworkBehaviour
{
    const string PlayerTag = "Player";

    public Action action;

    public override void Spawned()
    {
        // 무기마다, 무기를 소유한 플레이어 HandSocket에 다 붙인다
        AttachToOwner(Object.InputAuthority);
    }

    public void AttachToOwner(PlayerRef ownerRef)
    {
        // 태그 "Player"가 붙은 모든 플레이어 오브젝트 검색
        var players = GameObject.FindGameObjectsWithTag(PlayerTag);
        foreach (var go in players)
        {
            var psm = go.GetComponent<PlayerStateMachine>();
            // 이 플레이어가 weapon.InputAuthority와 같다면
            if (psm != null && psm.Object.InputAuthority == ownerRef)
            {   
                // 해당 플레이어의 오른손(또는 leftHandTransform)으로 parent
                transform.SetParent(psm.RightHandTransform, worldPositionStays: false);
                return;
            }
        }

        Debug.LogWarning($"[{name}] Owner({ownerRef}) 가진 플레이어를 찾을 수 없습니다.");
    }
}