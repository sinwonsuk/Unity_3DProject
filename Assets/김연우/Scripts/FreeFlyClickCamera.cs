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
        // 카메라 자동 할당
        if (cam == null) cam = GetComponent<Camera>();

        // 초기 Euler 값 저장
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;

        // 커서 잠금 & 숨김
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        // 1) 마우스로 시점 회전
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");
        yaw += mx * lookSpeed;
        pitch -= my * lookSpeed;
        pitch = Mathf.Clamp(pitch, -89f, 89f);
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

        // 2) 키보드로 이동
        Vector3 dir = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) dir += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) dir += Vector3.back;
        if (Input.GetKey(KeyCode.A)) dir += Vector3.left;
        if (Input.GetKey(KeyCode.D)) dir += Vector3.right;
        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;
        transform.position += transform.TransformDirection(dir.normalized) * speed * Time.deltaTime;

        // 3) (선택) 커서 잠금 토글 비활성화 주석 처리

        // 4) 좌클릭 시 월드 좌표 얻고, 디버그 + 프리팹 생성
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, groundLayer))
            {
                Vector3 worldPos = hit.point;
                Debug.Log($"클릭한 지점 월드 좌표: {worldPos}");

                // 디버그 레이(빨간색, 2초 동안)
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red, 2f);

                // 클릭 지점에 프리팹 설치
                if (clickPrefab != null)
                {
                    Instantiate(clickPrefab, worldPos, Quaternion.identity);
                }
                else
                {
                    Debug.LogWarning("ClickPrefab이 할당되지 않았습니다!");
                }
            }
        }
    }

    // 화면 중앙에 Crosshair 그리기
    void OnGUI()
    {
        if (crosshairTexture == null) return;

        float size = crosshairSize;
        float x = (Screen.width - size) * 0.5f;
        float y = (Screen.height - size) * 0.5f;
        GUI.DrawTexture(new Rect(x, y, size, size), crosshairTexture);
    }
}
