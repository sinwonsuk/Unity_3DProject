using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/SpawnManager")]
public class SpawnManagerConfig : BaseScriptableObject
{
    public SpawnManagerConfig()
    {
        type = typeof(SpawnManagerConfig);
    }

    [field: SerializeField]
    public List<Vector3> SpawnPositions { get; private set; }

    public List<Vector3> GetSpawnPositions()
    {
        return SpawnPositions;
    }
}
