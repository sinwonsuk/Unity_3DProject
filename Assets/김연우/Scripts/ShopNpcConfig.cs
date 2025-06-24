using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/ShopNpcConfig")]
public class ShopNpcConfig : ScriptableObject
{
    public List<Vector3> spawnPositions;
}