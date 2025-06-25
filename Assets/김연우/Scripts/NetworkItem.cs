// 2) NetworkItem.cs
using Fusion;
using UnityEngine;

public class NetworkItem : NetworkBehaviour
{
    // 기본 복제(Initial State)에만 사용 → OnChanged는 제거
    [Networked] public Vector3 SpawnPos { get; set; }

    public override void Spawned()
    {
        // 스폰 직후 한 번만 위치 반영
        transform.position = SpawnPos;
    }
}
