using Fusion;
using UnityEngine;

public class NetworkNPC : NetworkBehaviour
{
    [Networked] public Vector3 SpawnPos { get; set; }

    public override void Spawned()
    {
        // ���� ���� �� ���� ��ġ �ݿ�
        transform.position = SpawnPos;
    }
}
