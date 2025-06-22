using Fusion;
using static Unity.Collections.Unicode;
using UnityEngine;
using Unity.VisualScripting;

public class Arrow : NetworkBehaviour
{
    // ��Ʈ��ũ ������ ������Ƽ��
    [Networked] public bool flying { get; set; }
    [Networked] public Vector3 flyDir { get; set; }

    private float speed = 10.5f;

    public override void FixedUpdateNetwork()
    {

        if (!Object.HasStateAuthority && flying && Object.HasInputAuthority)
        {
            transform.position += Runner.DeltaTime * speed * flyDir;
        }

        else if (Object.HasStateAuthority && flying)
        {
            transform.position += Runner.DeltaTime * speed * flyDir;
        }
  
    
    }

    // �ݵ�� ȣ��Ʈ������ ȣ��
    public void Shoot(Vector3 dir)
    {
        flyDir = (dir - transform.position).normalized;

        flying = true;   // �� �� ���� flying�� flyDir �� �� ��Ʈ��ũ�� ����

        transform.SetParent(null, true);

        // ���ÿ��� ��� ���̵���
        transform.forward = flyDir;
        Destroy(gameObject, 10f);
    }
}