using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : baseManager, IGameManager
{
    private SpawnManagerConfig config;
    private List<Vector3> availableSpawnPositions;

    public SpawnManager(SpawnManagerConfig spawnConfig)
    {
        config = spawnConfig;
        type = typeof(SpawnManager);
        availableSpawnPositions = new List<Vector3>(config.GetSpawnPositions());
    }

    public SpawnManager(BaseScriptableObject baseScriptableObject)
    {
        type = typeof(SpawnManager);
        config = (SpawnManagerConfig)baseScriptableObject;
        availableSpawnPositions = new List<Vector3>(config.GetSpawnPositions());
    }

    public override void Init()
    {
        // 초기화 또는 로깅용
        Debug.Log($"SpawnManager initialized with {availableSpawnPositions.Count} spawn points.");
    }

    public Vector3 GetSpawnPosition()
    {
        if (availableSpawnPositions.Count == 0)
        {
            Debug.LogWarning("SpawnManager: No spawn positions available.");
            return Vector3.zero;
        }

        int index = Random.Range(0, availableSpawnPositions.Count);
        Vector3 pos = availableSpawnPositions[index];
        availableSpawnPositions.RemoveAt(index); // 중복 방지
        return pos;
    }
}
