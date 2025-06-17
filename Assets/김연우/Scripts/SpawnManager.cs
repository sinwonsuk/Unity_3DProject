using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private SpawnManagerConfig spawnData;
    private Queue<Vector3> spawnQueue;

    private void Awake()
    {
        spawnQueue = new Queue<Vector3>(spawnData.spawnPositions);
    }

    public Vector3 GetNextSpawnPosition()
    {
        return spawnQueue.Count > 0 ? spawnQueue.Dequeue() : Vector3.zero;
    }
}
