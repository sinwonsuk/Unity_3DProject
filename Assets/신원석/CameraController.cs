using UnityEngine;

public class CameraController : MonoBehaviour
{
    // 카메라가 따라다닐 대상 (일반적으로 플레이어)
    [SerializeField] Transform followTarget;

    // 카메라 회전 속도
    [SerializeField] float rotationSpeed = 2f;
    // 카메라와 타겟 사이의 거리
    [SerializeField] float distance = 5;

    // 수직 회전 제한 각도
    [SerializeField] float minVerticalAngle = -45;
    [SerializeField] float maxVerticalAngle = 45;

    // 카메라 위치 미세 조정을 위한 오프셋
    [SerializeField] Vector2 framingOffset;

    // 마우스 X축 반전 여부
    [SerializeField] bool invertX;
    // 마우스 Y축 반전 여부
    [SerializeField] bool invertY;

    // 현재 X축 회전값 (상하 회전)
    float rotationX;
    // 현재 Y축 회전값 (좌우 회전)
    float rotationY;

    // X축 반전 계수
    float invertXVal;
    // Y축 반전 계수
    float invertYVal;

    private void Start()
    {
        // 게임 시작시 마우스 커서 숨기고 고정
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // 마우스 반전 설정에 따른 계수 설정
        invertXVal = (invertX) ? -1 : 1;
        invertYVal = (invertY) ? -1 : 1;

        // 마우스 Y축 입력으로 상하 회전 처리
        rotationX += Input.GetAxis("Mouse Y") * invertYVal * rotationSpeed;
        rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);

        // 마우스 X축 입력으로 좌우 회전 처리
        rotationY += Input.GetAxis("Mouse X") * invertXVal * rotationSpeed;

        // 최종 회전값 계산
        var targetRotation = Quaternion.Euler(rotationX, rotationY, 0);

        // 카메라 위치 및 회전 적용
        //var focusPosition = followTarget.position + new Vector3(framingOffset.x, framingOffset.y, 0);
        //transform.position = focusPosition - targetRotation * new Vector3(0, 0, distance);
        transform.rotation = targetRotation;
    }

    // 플레이어 이동 방향 계산을 위한 Y축 회전값만 반환
    public Quaternion PlanarRotation => Quaternion.Euler(0, rotationY, 0);
}

