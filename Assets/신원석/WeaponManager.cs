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

    [Networked] public NetworkId CurrentMagicId { get; set; }
    [Networked] public HandSide Side { get; set; }

    [Networked] public HandSide ArrowSide { get; set; }

    [Networked] public NetworkObject currentWeapon { get; set; }
    [Networked] public NetworkObject Arrow { get; set; }

    [Networked] public NetworkObject Magic { get; set; }

    private ItemState currentWeaponState;

    [Networked] public ItemState magicState { get; set; }

    private NetworkRunner runner;

    private Queue<NetworkObject> IceMagics = new Queue<NetworkObject>();
    private Queue<NetworkObject> FireMagics = new Queue<NetworkObject>();
    private Queue<NetworkObject> ElectricMagics = new Queue<NetworkObject>();

    public void Init(WeaponsConfig config, Transform rightHandSocket, Transform leftHandSocket, NetworkRunner runner, NetworkBehaviour networkBehaviour)
    {
        this.config = config;
        this.rightHandSocket = rightHandSocket;
        this.leftHandSocket = leftHandSocket;
        this.runner = runner;
    }

    public void MagicInitialize(HandSide dir, PlayerRef owner)
    {
        for (int i = 0; i < 100; i++)
        {
            var obj = CreateMagics(ItemState.IceMagic, dir, owner);
            IceMagics.Enqueue(obj);
            //RPC_DeactivateMagic(obj.Id);
        }

        for (int i = 0; i < 100; i++)
        {
            var obj = CreateMagics(ItemState.FireMagic, dir, owner);
            FireMagics.Enqueue(obj);
           // RPC_DeactivateMagic(obj.Id);
        }

        for (int i = 0; i < 100; i++)
        {
            var obj = CreateMagics(ItemState.ElectricMagic, dir, owner);
            ElectricMagics.Enqueue(obj);
            //RPC_DeactivateMagic(obj.Id);
        }

    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_ActivateMagic(NetworkId id, RpcInfo info = default)
    {
        var networkObject = Runner.FindObject(id); 
        if (networkObject != null)
        {
            networkObject.gameObject.SetActive(true);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_DeactivateMagic(NetworkId id, RpcInfo info = default)
    {
        var networkObject = Runner.FindObject(id); 
        if (networkObject != null)
        {
            networkObject.gameObject.SetActive(false);
        }
    }


    public NetworkObject GetIceMagicPool()
    {
        // ����
        var obj = IceMagics.Dequeue();

        // ���� Ǯ�� ������Ʈ�� ���ٸ�
        if (obj == null)
        {
            Debug.LogError("Pool���� ������Ʈ�� ������ �� �����ϴ�.");
            return null;
        }
        // ������Ʈ�� Ȱ��ȭ�ϰ� ��ȯ
        obj.gameObject.SetActive(true);
        IceMagics.Enqueue(obj);
        RPC_ActivateMagic(obj.Id);
        return obj;
    }
    public NetworkObject GetFireMagicPool()
    {
        // ����
        var obj = FireMagics.Dequeue();

        // ���� Ǯ�� ������Ʈ�� ���ٸ�
        if (obj == null)
        {
            Debug.LogError("Pool���� ������Ʈ�� ������ �� �����ϴ�.");
            return null;
        }
        // ������Ʈ�� Ȱ��ȭ�ϰ� ��ȯ
        obj.gameObject.SetActive(true);
        FireMagics.Enqueue(obj);
        RPC_ActivateMagic(obj.Id);
        return obj;
    }
    public NetworkObject GetElectricMagicPool()
    {
        // ����
        var obj = ElectricMagics.Dequeue();

        // ���� Ǯ�� ������Ʈ�� ���ٸ�
        if (obj == null)
        {
            Debug.LogError("Pool���� ������Ʈ�� ������ �� �����ϴ�.");
            return null;
        }
        // ������Ʈ�� Ȱ��ȭ�ϰ� ��ȯ
        obj.gameObject.SetActive(true);
        ElectricMagics.Enqueue(obj);
        RPC_ActivateMagic(obj.Id);
        return obj;
    }

    public void Equip(ItemState state,HandSide Dir, PlayerRef owner = default)
    {

        // (1) �̹� ���Ⱑ ������ Despawn
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
            Debug.LogWarning("���� ����");
        }

        RPC_ActivateMagic(currentWeapon.Id);

        currentWeaponState = state;
    }
    public void CreateArrow(ItemState state, HandSide Dir, PlayerRef owner = default)
    {
        if (currentWeapon.GetComponent<Bow>() == null)
            Debug.Log("Ȱ���µ� ȭ�� ����");

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
            obj.transform.SetParent(rightHandSocket, false);
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
            obj.gameObject.SetActive(false);
            obj.gameObject.transform.SetParent(rightHandSocket, false);
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
        // ȣ��Ʈ�� ���� ����
        Equip(state, isDir, info.Source);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestArrow(ItemState state, HandSide isDir, RpcInfo info = default )
    {
        // ȣ��Ʈ�� ���� ����
        CreateArrow(state, isDir, info.Source);
    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestMagic(ItemState state, HandSide isDir, RpcInfo info = default)
    {
        // ȣ��Ʈ�� ���� ����
        CreateMagic(state, isDir, info.Source);
    }

    public void RequestEquip(ItemState state, HandSide dir, PlayerRef owner = default)
    {
        // (1) Ŭ���̾�Ʈ�� ���� RPC
        if (Object.HasInputAuthority && !Object.HasStateAuthority)
        {
            RPC_RequestEquip(state, dir);
        }
        // (2) ȣ��Ʈ�� ���� ��ٷ� ����
        else if (Object.HasStateAuthority)
        {
            Equip(state, dir, owner);
        }
    }
    public void RequestArrow(ItemState state, HandSide dir, PlayerRef owner = default)
    {
        // (1) Ŭ���̾�Ʈ�� ���� RPC
        if (Object.HasInputAuthority && !Object.HasStateAuthority)
        {
            RPC_RequestArrow(state, dir);
        }
        // (2) ȣ��Ʈ�� ���� ��ٷ� ����
        else if (Object.HasStateAuthority)
        {
            CreateArrow(state, dir, owner);
        }
    }

    public void RequestMagic(ItemState state, HandSide dir, PlayerRef owner = default)
    {
        // (1) Ŭ���̾�Ʈ�� ���� RPC
        if (Object.HasInputAuthority && !Object.HasStateAuthority)
        {
            RPC_RequestMagic(state, dir);
        }
        // (2) ȣ��Ʈ�� ���� ��ٷ� ����
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
