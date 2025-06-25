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
            Debug.LogError("[ItemSpawner] itemConfig�� �Ҵ���� �ʾҽ��ϴ�!");
            return;
        }

        foreach (var pos in itemConfig.spawnPositions)
        {
            var netObj = Runner.Spawn(itemPrefab, Vector3.zero, Quaternion.identity);
            if (netObj == null)
            {
                Debug.LogError("[ItemSpawner] Runner.Spawn�� null ��ȯ - Prefab ���� Ȯ��");
                continue;
            }

            if (netObj.HasStateAuthority)
            {
                var netItem = netObj.GetComponent<NetworkItem>();
                if (netItem == null)
                {
                    Debug.LogError("[ItemSpawner] Prefab�� NetworkItem ������Ʈ�� �����ϴ�!");
                    continue;
                }
                netItem.SpawnPos = pos;
            }
        }
    }

}
