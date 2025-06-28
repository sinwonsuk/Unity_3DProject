using Fusion;
using UnityEngine;

public class PlayerWeaponChanged : NetworkBehaviour
{
    string tagName = "Player";

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_ChangeWeapon(ItemState state,int num, RpcInfo info = default)
    {
        var target = FindPlayerByInputAuthority(info.Source);
        if (target != null)
        {
            PlayerRef adad = Object.InputAuthority;

            target.ApplyWeaponChange(state, info, adad, num);
        }
    }

    public void ChangeWeapon(ItemState state,int num)
    {
        if (Object.HasStateAuthority)
        {
            var inputAuthority = Object.InputAuthority;

            // 만약 PlayerRef.None이면 애초에 내 플레이어가 아님
            if (inputAuthority == PlayerRef.None)
            {
                Debug.LogError("[ChangeWeapon] InputAuthority is None on StateAuthority object!");
                return;
            }

            ApplyWeaponChange(state, default, inputAuthority,num);
        }
        else if (Object.HasInputAuthority)
        {
            RPC_ChangeWeapon(state, num);
        }
    }





    private PlayerWeaponChanged FindPlayerByInputAuthority(PlayerRef playerRef)
    {
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(tagName);

        foreach (var go in taggedObjects)
        {
            var stateMachine = go.GetComponent<PlayerStateMachine>();
            var weaponChanged = go.GetComponent<PlayerWeaponChanged>();

            if (stateMachine != null && weaponChanged != null)
            {
                var netObj = stateMachine.Object;
                if (netObj != null && netObj.InputAuthority == playerRef)
                {
                    return weaponChanged;
                }
            }
        }

        return null;
    }

    public void ApplyWeaponChange(ItemState state, RpcInfo info, PlayerRef info2,int num)
    {
        EventBus<WeaponChange>.Raise(new WeaponChange(state,info, info2,num));
    }
}