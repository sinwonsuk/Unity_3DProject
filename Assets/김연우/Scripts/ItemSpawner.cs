using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ItemSpawner : NetworkBehaviour
{
    [SerializeField] private ItemSpawnConfig itemConfig;
    [SerializeField] private NetworkPrefabRef itemPrefab; // ��Ʈ��ũ ������ ������

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
