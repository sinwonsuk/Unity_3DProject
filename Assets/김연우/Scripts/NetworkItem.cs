using Fusion;
using UnityEngine;

public class NetworkItem : NetworkBehaviour
{
    // �������� ������ ���� ��ġ�� Ŭ���̾�Ʈ�� ����ȭ�˴ϴ�.
    [Networked] public Vector3 SpawnPos { get; set; }

    public override void Spawned()
    {
        // ���� ���Ŀ� ��ġ�� �ݿ�
        transform.position = SpawnPos;
    }
}
