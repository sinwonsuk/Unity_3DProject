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
            // 1) 일단 (0,0,0)에 스폰
            var networkObject = Runner.Spawn(itemPrefab, Vector3.zero, Quaternion.identity);

            // 2) 서버 권한이 있을 때만 네트워크드 변수에 위치 저장
            if (networkObject.HasStateAuthority)
            {
                var netItem = networkObject.GetComponent<NetworkItem>();
                netItem.SpawnPos = pos;
            }
        }
    }
}
