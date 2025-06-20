using UnityEngine;
using Fusion;
using System;

public class WeaponNetworkObject : NetworkBehaviour
{
    const string PlayerTag = "Player";

    public Action action;

    public override void Spawned()
    {
        // ���⸶��, ���⸦ ������ �÷��̾� HandSocket�� �� ���δ�
        AttachToOwner(Object.InputAuthority);
    }

    public void AttachToOwner(PlayerRef ownerRef)
    {
        // �±� "Player"�� ���� ��� �÷��̾� ������Ʈ �˻�
        var players = GameObject.FindGameObjectsWithTag(PlayerTag);
        foreach (var go in players)
        {
            var psm = go.GetComponent<PlayerStateMachine>();
            // �� �÷��̾ weapon.InputAuthority�� ���ٸ�
            if (psm != null && psm.Object.InputAuthority == ownerRef)
            {   
                // �ش� �÷��̾��� ������(�Ǵ� leftHandTransform)���� parent
                transform.SetParent(psm.RightHandTransform, worldPositionStays: false);
                return;
            }
        }

        Debug.LogWarning($"[{name}] Owner({ownerRef}) ���� �÷��̾ ã�� �� �����ϴ�.");
    }
}