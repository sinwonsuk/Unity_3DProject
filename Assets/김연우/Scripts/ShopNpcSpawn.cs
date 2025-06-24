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
        foreach (var pos in shopNpcConfig.spawnPositions)
        {
            Runner.Spawn(itemPrefab, pos, Quaternion.identity);
        }
    }
}

