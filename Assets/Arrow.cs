using Fusion;
using static Unity.Collections.Unicode;
using UnityEngine;
using Unity.VisualScripting;
using static UnityEngine.UI.Image;

public class Arrow : NetworkBehaviour
{
    // ��Ʈ��ũ ������ ������Ƽ��
    [Networked] public bool flying { get; set; }
    [Networked] public Vector3 flyDir { get; set; }

    private float speed = 0.0f;

    public override void FixedUpdateNetwork()
    {



        if (Object.HasStateAuthority && flying)
        {
            transform.position += Runner.DeltaTime * speed * flyDir;
        }

        //Debug.DrawRay(transform.position, flyDir, Color.red);

        //Debug.Log(transform.position);
    }

    // �ݵ�� ȣ��Ʈ������ ȣ��
    public void Shoot(Vector3 dir)
    {
        //if (!Runner.IsForward || !Object.HasStateAuthority) return;

        //Bow bowComp = FindAnyObjectByType<Bow>();



        //transform.position = bowComp.GetComponent<Bow>().Rope.transform.position;

        Debug.Log($"�߻� ��ġ: {transform.position}");

        flying = true;   // �� �� ���� flying�� flyDir �� �� ��Ʈ��ũ�� ����

       
        flyDir = (dir - transform.position).normalized;

        transform.SetParent(null, true);
        // ���ÿ��� ��� ���̵���
        transform.forward = flyDir;
        Destroy(gameObject, 30f);
    }
}