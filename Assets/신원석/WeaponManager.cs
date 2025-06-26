using ExitGames.Client.Photon;
using Fusion;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static Fusion.NetworkRunner;
using static Unity.Collections.Unicode;
using static UnityEngine.UI.GridLayoutGroup;

public enum HandSide
{
    Right,
    Left,
}

public class WeaponManager : NetworkBehaviour 
{
    private WeaponsConfig config;
    private Transform rightHandSocket;
    private Transform leftHandSocket;


    [Networked] public HandSide Side { get; set; }

    [Networked] public HandSide ArrowSide { get; set; }

    [Networked] public NetworkObject currentWeapon { get; set; }
    [Networked] public NetworkObject Arrow { get; set; }

    [Networked] public NetworkObject Magic { get; set; }

    private ItemState currentWeaponState;

    [Networked] public ItemState magicState { get; set; }

    private NetworkRunner runner;

    private Queue<NetworkObject> IceMagics = new Queue<NetworkObject>();



    public void Init(WeaponsConfig config, Transform rightHandSocket, Transform leftHandSocket, NetworkRunner runner, NetworkBehaviour networkBehaviour)
    {
        this.config = config;
        this.rightHandSocket = rightHandSocket;
        this.leftHandSocket = leftHandSocket;
        this.runner = runner;
    }

    public void MaigcInitialize(ItemState state, HandSide Dir, PlayerRef owner = default)
    {
        for (int i = 0; i < 100; i++)
        {
            NetworkObject obj = CreateMagics(state, Dir, owner);

            obj.gameObject.SetActive(false);

            IceMagics.Enqueue(obj);

        }
    }
    public NetworkObject GetIceMagicPool()
    {
        // 디버깅
        var ad = Object.InputAuthority;


        // 뺏기
        var obj = IceMagics.Dequeue();

        // 만약 풀에 오브젝트가 없다면
        if (obj == null)
        {
            Debug.LogError("Pool에서 오브젝트를 가져올 수 없습니다.");
            return null;
        }
        // 오브젝트를 활성화하고 반환
        obj.gameObject.SetActive(true);

        IceMagics.Enqueue(obj);

        return obj;
    }


    public void Equip(ItemState state,HandSide Dir, PlayerRef owner = default)
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

        if (Dir == HandSide.Right)
        {
            Side = HandSide.Right;
            currentWeapon = runner.Spawn(prefab, position, rotation, owner);
        }
        else if(Dir == HandSide.Left)
        {
            Side = HandSide.Left;
            currentWeapon = runner.Spawn(prefab, position, rotation, owner);
        } 
        else
        {
            Debug.LogWarning("무기 없음");
        }

        //RPC_AttachWeaponToSocket(Dir);

        currentWeaponState = state;
    }
    public void CreateArrow(ItemState state, HandSide Dir, PlayerRef owner = default)
    {
        if (currentWeapon.GetComponent<Bow>() == null)
            Debug.Log("활없는데 화살 만듬");

        NetworkPrefabRef prefab = config.GetWeapon(state);
        Vector3 position = config.GetTransform(state).localPosition;
        Quaternion rotation = config.GetTransform(state).localRotation;
        NetworkObject obj = null;

        obj = runner.Spawn(prefab, position, rotation, owner, onBeforeSpawned: (runner, obj) =>
        {
            var wno = obj.GetComponent<WeaponNetworkArrow>();
            wno.RopeTransform = currentWeapon.GetComponent<Bow>().Rope.transform;
        }
            );

        Arrow = obj;
    }
    public void CreateMagic(ItemState state, HandSide Dir, PlayerRef owner = default)
    {
        magicState = state;

        NetworkPrefabRef prefab = config.GetWeapon(state);
        Vector3 position = config.GetTransform(state).localPosition;
        Quaternion rotation = config.GetTransform(state).localRotation;
        NetworkObject obj = null;

        obj = runner.Spawn(prefab, position, rotation, owner, onBeforeSpawned: (runner, obj) =>
        {
            var wno = obj.GetComponent<WeaponNetworkObject>();
            
        }
            );

        Magic = obj;
    }


    public NetworkObject CreateMagics(ItemState state, HandSide Dir, PlayerRef owner = default)
    {
        magicState = state;

        NetworkPrefabRef prefab = config.GetWeapon(state);
        Vector3 position = config.GetTransform(state).localPosition;
        Quaternion rotation = config.GetTransform(state).localRotation;
        NetworkObject obj = null;

        obj = runner.Spawn(prefab, position, rotation, owner, onBeforeSpawned: (runner, obj) =>
        {
            var wno = obj.GetComponent<WeaponNetworkObject>();

        }
            );

        return obj;
    }






    public NetworkObject GetCreateMagic(ItemState state, HandSide Dir, PlayerRef owner = default)
    {
        magicState = state;

        NetworkPrefabRef prefab = config.GetWeapon(state);
        Vector3 position = config.GetTransform(state).localPosition;
        Quaternion rotation = config.GetTransform(state).localRotation;
        NetworkObject obj = null;

        obj = runner.Spawn(prefab, position, rotation, owner, onBeforeSpawned: (runner, obj) =>
        {
            var wno = obj.GetComponent<WeaponNetworkObject>();

        }
            );

        return obj;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestEquip(ItemState state, HandSide isDir, RpcInfo info = default)
    {
        // 호스트만 여기 진입
        Equip(state, isDir, info.Source);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestArrow(ItemState state, HandSide isDir, RpcInfo info = default )
    {
        // 호스트만 여기 진입
        CreateArrow(state, isDir, info.Source);
    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestMagic(ItemState state, HandSide isDir, RpcInfo info = default)
    {
        // 호스트만 여기 진입
        CreateMagic(state, isDir, info.Source);
    }

    public void RequestEquip(ItemState state, HandSide dir, PlayerRef owner = default)
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
    public void RequestArrow(ItemState state, HandSide dir, PlayerRef owner = default)
    {
        // (1) 클라이언트일 때만 RPC
        if (Object.HasInputAuthority && !Object.HasStateAuthority)
        {
            RPC_RequestArrow(state, dir);
        }
        // (2) 호스트일 때는 곧바로 실행
        else if (Object.HasStateAuthority)
        {
            CreateArrow(state, dir, owner);
        }
    }

    public void RequestMagic(ItemState state, HandSide dir, PlayerRef owner = default)
    {
        // (1) 클라이언트일 때만 RPC
        if (Object.HasInputAuthority && !Object.HasStateAuthority)
        {
            RPC_RequestMagic(state, dir);
        }
        // (2) 호스트일 때는 곧바로 실행
        else if (Object.HasStateAuthority)
        {
            CreateMagic(state, dir, owner);
        }
    }

    public void Unequip()
    {
        if (currentWeapon != null)
        {
            GameObject.Destroy(currentWeapon);
            currentWeapon = null;
        }
    }
}
