using UnityEngine;

public class SpectatorCameraController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 8f;
    [SerializeField] float lookSpeed = 3f;

    public static void Spawn(Vector3 pos, Quaternion rot)
    {
        var prefab = Resources.Load<SpectatorCameraController>("SpectatorCamera");
        Instantiate(prefab, pos, rot);
    }

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // 이동
        Vector3 dir = new(
            Input.GetAxisRaw("Horizontal"),
            (Input.GetKey(KeyCode.E) ? 1 : 0) - (Input.GetKey(KeyCode.Q) ? 1 : 0),
            Input.GetAxisRaw("Vertical"));
        transform.position += transform.rotation * dir.normalized * moveSpeed * Time.deltaTime;

        // 회전
        float mx = Input.GetAxis("Mouse X") * lookSpeed;
        float my = Input.GetAxis("Mouse Y") * lookSpeed;
        transform.rotation *= Quaternion.Euler(-my, mx, 0);
    }
}
