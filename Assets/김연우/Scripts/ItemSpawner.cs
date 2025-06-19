using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ItemSpawner : NetworkBehaviour
{
    [SerializeField] private ItemSpawnConfig itemConfig;
    [SerializeField] private NetworkPrefabRef itemPrefab; // 네트워크 아이템 프리팹

    public override void Spawned()
    {
        if (Runner.IsServer)
        {
            SpawnItems();
        }
    }

    private void SpawnItems()
    {
        foreach (var pos in itemConfig.spawnPositions)
        {
            Runner.Spawn(itemPrefab, pos, Quaternion.identity);
        }
    }
}
