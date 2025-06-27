using UnityEngine;

public class SpectatorCameraController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 8f;
    [SerializeField] float lookSpeed = 3f;

    float pitch = 0f; // 상하 회전
    float yaw = 0f;   // 좌우 회전

    public static void Spawn(Vector3 pos, Quaternion rot)
    {
        var prefab = Resources.Load<SpectatorCameraController>("SpectatorCamera");
        Instantiate(prefab, pos, rot);
    }

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Vector3 angles = transform.eulerAngles;
        pitch = angles.x;
        yaw = angles.y;
    }

    void Update()
    {
        // 마우스로 회전 (상하 + 좌우)
        float mx = Input.GetAxis("Mouse X") * lookSpeed;
        float my = Input.GetAxis("Mouse Y") * lookSpeed;

        yaw += mx;
        pitch -= my;
        pitch = Mathf.Clamp(pitch, -89f, 89f); // 상하 회전 제한

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

        // WASD 이동 (카메라 기준 방향)
        Vector3 moveDir = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) moveDir += transform.forward;
        if (Input.GetKey(KeyCode.S)) moveDir -= transform.forward;
        if (Input.GetKey(KeyCode.A)) moveDir -= transform.right;
        if (Input.GetKey(KeyCode.D)) moveDir += transform.right;

        //moveDir.y = 0f; // 수직 이동 제거 (지면과 평행 이동만)
        transform.position += moveDir.normalized * moveSpeed * Time.deltaTime;
    }
}
