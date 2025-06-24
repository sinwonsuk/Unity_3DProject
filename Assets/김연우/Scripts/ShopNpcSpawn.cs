using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ShopNpcSpawn : NetworkBehaviour
{
    [SerializeField] private ShopNpcConfig shopNpcConfig;
    [SerializeField] private NetworkPrefabRef itemPrefab; // 네트워크 아이템 프리팹

    public override void Spawned()
    {
        if (Runner.IsServer)
        {
            SpawnNPC();
        }
    }

    private void SpawnNPC()
    {
        foreach (var pos in shopNpcConfig.spawnPositions)
        {
            Runner.Spawn(itemPrefab, pos, Quaternion.identity);
        }
    }
}

