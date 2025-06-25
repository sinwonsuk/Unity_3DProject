using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ItemSpawner : NetworkBehaviour
{
    [SerializeField] private ItemSpawnConfig itemConfig;
    [SerializeField] private NetworkPrefabRef itemPrefab;

    public override void Spawned()
    {
        if (Runner.IsServer)
            SpawnItems();
    }
    private void SpawnItems()
    {
        if (itemConfig == null) return;

        foreach (var pos in itemConfig.spawnPositions)
        {
            Runner.Spawn(
                itemPrefab,
                Vector3.zero,
                Quaternion.identity,
                default,  
                (runner, netObj) =>
                {
                    var netItem = netObj.GetComponent<NetworkItem>();
                    if (netItem != null)
                        netItem.SpawnPos = pos;
                }
            );
        }
    }





}
