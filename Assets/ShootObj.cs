using Fusion;
using static Unity.Collections.Unicode;
using UnityEngine;
using Unity.VisualScripting;
using static UnityEngine.UI.Image;

public class ShootObj : NetworkBehaviour
{
    // ��Ʈ��ũ ������ ������Ƽ��
    [Networked] public bool flying { get; set; }
    [Networked] public Vector3 flyDir { get; set; }

 
    [SerializeField] private float speed = 0.0f;

    public override void FixedUpdateNetwork()
    {

        if (flying)
        {
            transform.position += Runner.DeltaTime * speed * flyDir;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        flying = false;
    }



    public void Shoot(Vector3 dir)
    {
        Debug.Log($"�߻� ��ġ: {transform.position}");

        flying = true;   // �� �� ���� flying�� flyDir �� �� ��Ʈ��ũ�� ����

       
        flyDir = (dir - transform.position).normalized;

        transform.SetParent(null, true);
        // ���ÿ��� ��� ���̵���
        transform.forward = flyDir;
        Destroy(gameObject, 10f);
    }
}