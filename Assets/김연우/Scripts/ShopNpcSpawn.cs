using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ShopNpcSpawn : NetworkBehaviour
{
    [SerializeField] private ShopNpcConfig shopNpcConfig;
    [SerializeField] private NetworkPrefabRef itemPrefab; // ��Ʈ��ũ ������ ������

    public override void Spawned()
    {
        if (Runner.IsServer)
        {
            SpawnNPC();
        }
    }

    private void SpawnNPC()
    {
        if (shopNpcConfig == null) return;

        foreach (var pos in shopNpcConfig.spawnPositions)
        {
            Runner.Spawn(
                itemPrefab,
                Vector3.zero,
                Quaternion.identity,
                default,
                (runner, netObj) =>
                {
                    var netItem = netObj.GetComponent<NetworkNPC>();
                    if (netItem != null)
                        netItem.SpawnPos = pos;
                }
            );
        }
    }
}

