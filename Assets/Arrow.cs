using Fusion;
using static Unity.Collections.Unicode;
using UnityEngine;
using Unity.VisualScripting;

public class Arrow : NetworkBehaviour
{
    // 네트워크 복제할 프로퍼티들
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

    // 반드시 호스트에서만 호출
    public void Shoot(Vector3 dir)
    {
        flyDir = (dir - transform.position).normalized;

        flying = true;   // ← 이 순간 flying과 flyDir 둘 다 네트워크로 복제

        transform.SetParent(null, true);

        // 로컬에서 즉시 보이도록
        transform.forward = flyDir;
        Destroy(gameObject, 10f);
    }
}