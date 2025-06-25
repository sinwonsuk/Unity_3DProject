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
        foreach (var pos in itemConfig.spawnPositions)
        {
            // 1) �ϴ� (0,0,0)�� ����
            var networkObject = Runner.Spawn(itemPrefab, Vector3.zero, Quaternion.identity);

            // 2) ���� ������ ���� ���� ��Ʈ��ũ�� ������ ��ġ ����
            if (networkObject.HasStateAuthority)
            {
                var netItem = networkObject.GetComponent<NetworkItem>();
                netItem.SpawnPos = pos;
            }
        }
    }
}
