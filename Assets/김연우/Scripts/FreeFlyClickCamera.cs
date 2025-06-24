using UnityEngine;

public class FreeFlyClickCamera : MonoBehaviour
{
    public Camera cam;

    public float moveSpeed = 10f;
    public float sprintSpeed = 30f;
    public float lookSpeed = 2f;
    public bool lockCursor = true;
    public LayerMask groundLayer;
    public float maxDistance = 100f;

    public Texture2D crosshairTexture;
    public float crosshairSize = 16f;
    public GameObject clickPrefab;

    float yaw;
    float pitch;

    void Awake()
    {
        // ī�޶� �ڵ� �Ҵ�
        if (cam == null) cam = GetComponent<Camera>();

        // �ʱ� Euler �� ����
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;

        // Ŀ�� ��� & ����
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        // 1) ���콺�� ���� ȸ��
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");
        yaw += mx * lookSpeed;
        pitch -= my * lookSpeed;
        pitch = Mathf.Clamp(pitch, -89f, 89f);
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

        // 2) Ű����� �̵�
        Vector3 dir = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) dir += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) dir += Vector3.back;
        if (Input.GetKey(KeyCode.A)) dir += Vector3.left;
        if (Input.GetKey(KeyCode.D)) dir += Vector3.right;
        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;
        transform.position += transform.TransformDirection(dir.normalized) * speed * Time.deltaTime;

        // 3) (����) Ŀ�� ��� ��� ��Ȱ��ȭ �ּ� ó��

        // 4) ��Ŭ�� �� ���� ��ǥ ���, ����� + ������ ����
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, groundLayer))
            {
                Vector3 worldPos = hit.point;
                Debug.Log($"Ŭ���� ���� ���� ��ǥ: {worldPos}");

                // ����� ����(������, 2�� ����)
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red, 2f);

                // Ŭ�� ������ ������ ��ġ
                if (clickPrefab != null)
                {
                    Instantiate(clickPrefab, worldPos, Quaternion.identity);
                }
                else
                {
                    Debug.LogWarning("ClickPrefab�� �Ҵ���� �ʾҽ��ϴ�!");
                }
            }
        }
    }

    // ȭ�� �߾ӿ� Crosshair �׸���
    void OnGUI()
    {
        if (crosshairTexture == null) return;

        float size = crosshairSize;
        float x = (Screen.width - size) * 0.5f;
        float y = (Screen.height - size) * 0.5f;
        GUI.DrawTexture(new Rect(x, y, size, size), crosshairTexture);
    }
}
