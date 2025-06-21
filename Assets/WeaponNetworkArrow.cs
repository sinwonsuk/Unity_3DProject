using Fusion;
using UnityEngine;

public class WeaponNetworkArrow : NetworkBehaviour
{

    const string PlayerTag = "Player";

    public Transform RopeTransform { get; set; }
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
                var bow = psm.GetComponent<PlayerStateMachine>()
                           .WeaponManager
                           .currentWeapon
                           .GetComponent<Bow>();
                var rope = bow.Rope.transform;

                transform.SetParent(rope, worldPositionStays: false);
                return;
            }
        }


    }
}
