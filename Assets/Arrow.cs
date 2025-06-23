using Fusion;
using static Unity.Collections.Unicode;
using UnityEngine;
using Unity.VisualScripting;
using static UnityEngine.UI.Image;

public class Arrow : NetworkBehaviour
{
    // 네트워크 복제할 프로퍼티들
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

    // 반드시 호스트에서만 호출
    public void Shoot(Vector3 dir)
    {
        //if (!Runner.IsForward || !Object.HasStateAuthority) return;

        //Bow bowComp = FindAnyObjectByType<Bow>();



        //transform.position = bowComp.GetComponent<Bow>().Rope.transform.position;

        Debug.Log($"발사 위치: {transform.position}");

        flying = true;   // ← 이 순간 flying과 flyDir 둘 다 네트워크로 복제

       
        flyDir = (dir - transform.position).normalized;

        transform.SetParent(null, true);
        // 로컬에서 즉시 보이도록
        transform.forward = flyDir;
        Destroy(gameObject, 30f);
    }
}