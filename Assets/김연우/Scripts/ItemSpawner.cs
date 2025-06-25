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
        if (itemConfig == null)
        {
            Debug.LogError("[ItemSpawner] itemConfig가 할당되지 않았습니다!");
            return;
        }

        foreach (var pos in itemConfig.spawnPositions)
        {
            var netObj = Runner.Spawn(itemPrefab, Vector3.zero, Quaternion.identity);
            if (netObj == null)
            {
                Debug.LogError("[ItemSpawner] Runner.Spawn이 null 반환 - Prefab 설정 확인");
                continue;
            }

            if (netObj.HasStateAuthority)
            {
                var netItem = netObj.GetComponent<NetworkItem>();
                if (netItem == null)
                {
                    Debug.LogError("[ItemSpawner] Prefab에 NetworkItem 컴포넌트가 없습니다!");
                    continue;
                }
                netItem.SpawnPos = pos;
            }
        }
    }

}
