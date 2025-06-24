using Fusion;
using UnityEngine;

public class WeaponNetworkMagic : NetworkBehaviour
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
            // �� �÷��̾ weapon.InputAuthority�� ���ٸ�
            if (psm != null && psm.Object.InputAuthority == ownerRef)
            {
                var magoc = psm.GetComponent<PlayerStateMachine>()
                           .WeaponManager
                           .currentWeapon
                           .GetComponent<Bow>();
     
                return;
            }
        }


    }
}
