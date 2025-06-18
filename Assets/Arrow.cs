using UnityEngine;

public class Arrow : MonoBehaviour
{
    LayerMask mask;
    Vector3 dir;
    Ray ray;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, 10f);
    }

    // Update is called once per frame
    void Update()
    {

        if (!flying) return;

        transform.position += flyDir * speed * Time.deltaTime;

        Debug.DrawRay(ray.origin, ray.direction * 999f, Color.red);
    }

    //private void OnTriggerEnter(Collision collision)
    //{
    //    transform.SetParent(collision.transform);
    //    gameObject.GetComponent<MeshCollider>().enabled = false;
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Object"))
        {
            gameObject.GetComponent<BoxCollider>().enabled = false;
            flying = false;
        }

        //transform.SetParent(other.transform);
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    void OnDrawGizmos()
    {
        if (Camera.main == null) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(ray.origin, ray.direction * 10f);
    }
    public void Shoot(Vector3 pos)
    {
        Vector3 dir = pos - transform.position;

        flying = true;
        flyDir = dir.normalized;
        transform.SetParent(null);              // 부모 분리
        transform.forward = flyDir;             // 화살 방향
    }

    private Vector3 flyDir;
    private bool flying = false;
    private float speed = 1.0f;
}
