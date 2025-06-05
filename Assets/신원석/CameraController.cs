using UnityEngine;

public class CameraController : MonoBehaviour
{
    // ī�޶� ����ٴ� ��� (�Ϲ������� �÷��̾�)
    [SerializeField] Transform followTarget;

    // ī�޶� ȸ�� �ӵ�
    [SerializeField] float rotationSpeed = 2f;
    // ī�޶�� Ÿ�� ������ �Ÿ�
    [SerializeField] float distance = 5;

    // ���� ȸ�� ���� ����
    [SerializeField] float minVerticalAngle = -45;
    [SerializeField] float maxVerticalAngle = 45;

    // ī�޶� ��ġ �̼� ������ ���� ������
    [SerializeField] Vector2 framingOffset;

    // ���콺 X�� ���� ����
    [SerializeField] bool invertX;
    // ���콺 Y�� ���� ����
    [SerializeField] bool invertY;

    // ���� X�� ȸ���� (���� ȸ��)
    float rotationX;
    // ���� Y�� ȸ���� (�¿� ȸ��)
    float rotationY;

    // X�� ���� ���
    float invertXVal;
    // Y�� ���� ���
    float invertYVal;

    private void Start()
    {
        // ���� ���۽� ���콺 Ŀ�� ����� ����
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // ���콺 ���� ������ ���� ��� ����
        invertXVal = (invertX) ? -1 : 1;
        invertYVal = (invertY) ? -1 : 1;

        // ���콺 Y�� �Է����� ���� ȸ�� ó��
        rotationX += Input.GetAxis("Mouse Y") * invertYVal * rotationSpeed;
        rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);

        // ���콺 X�� �Է����� �¿� ȸ�� ó��
        rotationY += Input.GetAxis("Mouse X") * invertXVal * rotationSpeed;

        // ���� ȸ���� ���
        var targetRotation = Quaternion.Euler(rotationX, rotationY, 0);

        // ī�޶� ��ġ �� ȸ�� ����
        //var focusPosition = followTarget.position + new Vector3(framingOffset.x, framingOffset.y, 0);
        //transform.position = focusPosition - targetRotation * new Vector3(0, 0, distance);
        transform.rotation = targetRotation;
    }

    // �÷��̾� �̵� ���� ����� ���� Y�� ȸ������ ��ȯ
    public Quaternion PlanarRotation => Quaternion.Euler(0, rotationY, 0);
}

