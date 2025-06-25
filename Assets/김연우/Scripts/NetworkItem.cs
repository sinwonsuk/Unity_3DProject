using Fusion;
using UnityEngine;

public class NetworkItem : NetworkBehaviour
{
    // 서버에서 세팅한 스폰 위치가 클라이언트로 동기화됩니다.
    [Networked] public Vector3 SpawnPos { get; set; }

    public override void Spawned()
    {
        // 스폰 직후에 위치를 반영
        transform.position = SpawnPos;
    }
}
