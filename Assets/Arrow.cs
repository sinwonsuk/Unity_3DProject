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

    public override void FixedUpdateNetwork()
    {
        if (flying)
        {
            // detachment
            if (transform.parent != null)
                transform.SetParent(null, true);

            // 호스트·클라이언트 모두 이 라인에서 같이 이동
            transform.position += Runner.DeltaTime * speed * flyDir;
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

        // Visual detach immediately
        gameObject.SetActive(true);
        transform.SetParent(null, true);
        transform.forward = flyDir;

    }
}
