using Fusion;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class Arrow : NetworkBehaviour
{
    [Networked] public bool flying { get; private set; }
    [Networked] public Vector3 flyDir { get; private set; }

    [SerializeField] private float speed = 10f;

    BoxCollider boxCollider;

    [SerializeField] private LayerMask hitLayers;
    [SerializeField] private int attackDamage;
    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    public override void FixedUpdateNetwork()
    {


        if (!Object.HasStateAuthority)
            return;

        Vector3 origin = transform.position;
        Vector3 displacement = flyDir * speed * Runner.DeltaTime;
        float distance = displacement.magnitude;

        if (Physics.Raycast(origin, flyDir, out RaycastHit hit, distance, hitLayers, QueryTriggerInteraction.Ignore))
        {

            // (b) �浹 ��� ���� ó��
            int layer = hit.collider.gameObject.layer;
            int playerLayer = LayerMask.NameToLayer("Player");
            int groundLayer = LayerMask.NameToLayer("Ground");

            if (layer == playerLayer)
            {
                var ui = hit.collider.transform.parent.GetComponent<PlayerHealth>();
                if (ui != null)
                    ui.TakeDamage(attackDamage);
            }

            // (c) ���� ��ų� �÷��̾ ������ ���߱�
            flying = false;
            Destroy(gameObject);
        }

        if (flying)
        {
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
                transform.SetParent(null, true);               
            }
        }
    }

    //public void OnTriggerEnter(Collider collider)
    //{


    //    // 1) ȣ��Ʈ������ �浹 ó��
    //    if (!Object.HasStateAuthority)
    //        return;

    //    int playerLayer = LayerMask.NameToLayer("Player");
    //    int groundLayer = LayerMask.NameToLayer("Ground");

    //    // 2) �浹�� ������Ʈ�� ���̾ �ٸ��� ����
    //    if (collider.gameObject.layer == playerLayer)
    //    {
    //        PlayerHealth UI = collider.transform.parent.GetComponent<PlayerHealth>();

    //        if (UI != null)
    //        {
    //            Debug.Log("�浹 ����!2");
    //            UI.TakeDamages(20);
    //        }
    //    }

    //}

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
