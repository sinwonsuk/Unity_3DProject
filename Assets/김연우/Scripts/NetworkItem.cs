// 2) NetworkItem.cs
using Fusion;
using UnityEngine;

public class NetworkItem : NetworkBehaviour
{
    // �⺻ ����(Initial State)���� ��� �� OnChanged�� ����
    [Networked] public Vector3 SpawnPos { get; set; }

    public override void Spawned()
    {
        // ���� ���� �� ���� ��ġ �ݿ�
        transform.position = SpawnPos;
    }
}
