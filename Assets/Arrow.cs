using Fusion;
using static Unity.Collections.Unicode;
using UnityEngine;
using Unity.VisualScripting;
using static UnityEngine.UI.Image;
using System.Collections;
using System.Collections.Generic;

public class Arrow : NetworkBehaviour
{
    [Networked] public bool flying { get; private set; }
    [Networked] public Vector3 flyDir { get; private set; }

    [SerializeField] private float speed = 10f;

    BoxCollider boxCollider;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }


    public override void FixedUpdateNetwork()
    {
        if (flying)
        {
            // ȣ��Ʈ��Ŭ���̾�Ʈ ��� �� ���ο��� ���� �̵�
            transform.position += Runner.DeltaTime * speed * flyDir;
        }
    }

    public override void Render()
    {

        if (flying)
        {
            // detachment
            if (transform.parent != null)
            {
                boxCollider.enabled = true;
                transform.SetParent(null, true);
            }
        }
    }

    public void OnTriggerEnter(Collider collider)
    {


        // 1) ȣ��Ʈ������ �浹 ó��
        if (!Object.HasStateAuthority)
            return;

        int playerLayer = LayerMask.NameToLayer("Player");
        int groundLayer = LayerMask.NameToLayer("Ground");

        // 2) �浹�� ������Ʈ�� ���̾ �ٸ��� ����
        if (collider.gameObject.layer == playerLayer)
        {
            PlayerHealth UI = collider.transform.parent.GetComponent<PlayerHealth>();

            if (UI != null)
            {
                Debug.Log("�浹 ����!2");
                UI.TakeDamages(20);
            }
        }

        if (collider.gameObject.layer == groundLayer)
        {
            flying = false;
        }
    }

    public void MagicShoot(Vector3 dir)
    {
        if (Object.HasStateAuthority)
        {
            flying = true;
            flyDir = (dir - transform.position).normalized;
        }

        // Visual detach immediately
        boxCollider.enabled = true;
        gameObject.SetActive(true);
        transform.SetParent(null, true);
        transform.forward = flyDir;
    }

    public void ArrowShoot(Vector3 dir)
    {
        if (Object.HasStateAuthority)
        {
            flying = true;
            flyDir = (dir - transform.position).normalized;
        }
        
        transform.SetParent(null, true);
        transform.forward = flyDir;
        Destroy(gameObject, 10.0f);
    }
}
