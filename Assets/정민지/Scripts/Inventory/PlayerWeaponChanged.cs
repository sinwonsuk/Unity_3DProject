using Fusion;
using UnityEngine;

public class PlayerWeaponChanged : NetworkBehaviour
{
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_ChangeWeapon(ItemState state, RpcInfo info = default)
    {
        EventBus<WeaponChange>.Raise(new WeaponChange(state));
    }
}
