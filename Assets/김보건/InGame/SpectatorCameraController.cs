using UnityEngine;

public class SpectatorCameraController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 8f;
    [SerializeField] float lookSpeed = 3f;

    float pitch = 0f; // ���� ȸ��
    float yaw = 0f;   // �¿� ȸ��

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
        // ���콺�� ȸ�� (���� + �¿�)
        float mx = Input.GetAxis("Mouse X") * lookSpeed;
        float my = Input.GetAxis("Mouse Y") * lookSpeed;

        yaw += mx;
        pitch -= my;
        pitch = Mathf.Clamp(pitch, -89f, 89f); // ���� ȸ�� ����

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

        // WASD �̵� (ī�޶� ���� ����)
        Vector3 moveDir = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) moveDir += transform.forward;
        if (Input.GetKey(KeyCode.S)) moveDir -= transform.forward;
        if (Input.GetKey(KeyCode.A)) moveDir -= transform.right;
        if (Input.GetKey(KeyCode.D)) moveDir += transform.right;

        //moveDir.y = 0f; // ���� �̵� ���� (����� ���� �̵���)
        transform.position += moveDir.normalized * moveSpeed * Time.deltaTime;
    }
}
