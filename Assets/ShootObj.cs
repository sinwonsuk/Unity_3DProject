using Fusion;
using static Unity.Collections.Unicode;
using UnityEngine;
using Unity.VisualScripting;
using static UnityEngine.UI.Image;

public class ShootObj : NetworkBehaviour
{
    // 네트워크 복제할 프로퍼티들
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
        Debug.Log($"발사 위치: {transform.position}");

        flying = true;   // ← 이 순간 flying과 flyDir 둘 다 네트워크로 복제

       
        flyDir = (dir - transform.position).normalized;

        transform.SetParent(null, true);
        // 로컬에서 즉시 보이도록
        transform.forward = flyDir;
        Destroy(gameObject, 10f);
    }
}