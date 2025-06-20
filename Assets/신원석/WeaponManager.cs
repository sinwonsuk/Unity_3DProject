using Fusion;
using UnityEngine;
using static Unity.Collections.Unicode;

public enum isDir
{
    Right,
    Left,
}

public class WeaponManager : NetworkBehaviour
{
    private WeaponsConfig config;
    private Transform rightHandSocket;
    private Transform leftHandSocket;
    private NetworkObject currentWeapon;
    private ItemState currentWeaponState;
    private NetworkRunner runner;
    private NetworkBehaviour networkBehaviour;

    public void Init(WeaponsConfig config, Transform rightHandSocket, Transform leftHandSocket, NetworkRunner runner, NetworkBehaviour networkBehaviour)
    {
        this.config = config;
        this.rightHandSocket = rightHandSocket;
        this.leftHandSocket = leftHandSocket;
        this.runner = runner;
        this.networkBehaviour = networkBehaviour;
    }


    public void Equip(ItemState state,isDir Dir, PlayerRef owner = default)
    {

        // (1) 이미 무기가 있으면 Despawn
        if (currentWeapon != null)
        {
            runner.Despawn(currentWeapon);
            currentWeapon = null;
        }

        NetworkPrefabRef prefab = config.GetWeapon(state);
        Vector3 position = config.GetTransform(state).localPosition;
        Quaternion rotation = config.GetTransform(state).localRotation;

        if (Dir == isDir.Right)
        {
            currentWeapon = runner.Spawn(prefab, position, rotation, owner);
        }
        else if(Dir == isDir.Left)
        {
            currentWeapon = runner.Spawn(prefab, position, rotation, owner);
        } 
        else
        {
            Debug.LogWarning("무기 없음");
        }

        //RPC_AttachWeaponToSocket(Dir);

        currentWeaponState = state;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestEquip(ItemState state, isDir isDir, RpcInfo info = default)
    {
        // 호스트만 여기 진입
        Equip(state, isDir, info.Source);
    }

    public void AttachWeaponToSocket(/*isDir Dir*/)
    {
        //if (Dir == isDir.Right)
            currentWeapon.transform.SetParent(rightHandSocket, false);
        //else
        //    currentWeapon.transform.SetParent(leftHandSocket, false);
    }   

    public void RequestEquip(ItemState state, isDir dir, PlayerRef owner = default)
    {
        // (1) 클라이언트일 때만 RPC
        if (Object.HasInputAuthority && !Object.HasStateAuthority)
        {
            RPC_RequestEquip(state, dir);
        }
        // (2) 호스트일 때는 곧바로 실행
        else if (Object.HasStateAuthority)
        {
            Equip(state, dir, owner);
        }
    }



    public GameObject CreateArrow()
    {
        if (currentWeaponState != ItemState.Bow)
            return null;

        return null;    
    }
    public GameObject CreateMagic()
    {
        if (currentWeaponState != ItemState.Magic)
            return null;

        return null;
    }

    public void Unequip()
    {
        if (currentWeapon != null)
        {
            GameObject.Destroy(currentWeapon);
            currentWeapon = null;
        }
    }
    public NetworkObject GetCurrentWeapon() => currentWeapon;
}