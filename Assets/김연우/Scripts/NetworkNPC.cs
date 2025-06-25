using Fusion;
using UnityEngine;

public class NetworkNPC : NetworkBehaviour
{
    [Networked] public Vector3 SpawnPos { get; set; }

    public override void Spawned()
    {
        // 스폰 직후 한 번만 위치 반영
        transform.position = SpawnPos;
    }
}
