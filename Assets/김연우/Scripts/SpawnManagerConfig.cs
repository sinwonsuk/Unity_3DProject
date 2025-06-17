using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/SpawnPositionData")]
public class SpawnManagerConfig : ScriptableObject
{
    public List<Vector3> spawnPositions;
}
