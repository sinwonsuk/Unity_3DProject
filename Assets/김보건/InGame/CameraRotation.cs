using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    float horizontalRot;
    float verticalRot;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float sensitivity = DpiSetting.Sensitivity;

        float mx = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float my = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        horizontalRot += mx;
        verticalRot -= my;
        verticalRot = Mathf.Clamp(verticalRot, -90f, 90f);

        transform.rotation = Quaternion.Euler(verticalRot, horizontalRot, 0f);
    }
}
