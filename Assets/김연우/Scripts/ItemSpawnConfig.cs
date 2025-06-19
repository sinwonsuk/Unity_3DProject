using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/ItemSpawnConfig")]
public class ItemSpawnConfig : ScriptableObject
{
    public List<Vector3> spawnPositions;
}
