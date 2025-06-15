using Fusion;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField]
    private Transform followTarget;
    [SerializeField]
    private float rotationSpeed = 10.0f;
    [SerializeField]
    private float topClamp = 70.0f;
    [SerializeField]
    private float bottomClamp = -40.0f;

    float cinemachineTargetYaw;
    float cinemachineTargetPitch;

    public static float CachedYaw { get; private set; } 
    bool mousecheck = false;

    public void LateUpdate()
    {
        CameraLogic();
    }

    private void CameraLogic()
    {
        float mouseX = GetMouseInput("Mouse X");
        float mouseY = GetMouseInput("Mouse Y");

        cinemachineTargetPitch = UpdateRotation(cinemachineTargetPitch, mouseY, bottomClamp, topClamp, true);
        cinemachineTargetYaw = UpdateRotation(cinemachineTargetYaw, mouseX, float.MinValue, float.MaxValue, true);

        ApplyRotation(cinemachineTargetPitch, cinemachineTargetYaw);

        CachedYaw = cinemachineTargetYaw;
    }

    private void ApplyRotation(float pitch, float yaw)
    {
        followTarget.rotation = Quaternion.Euler(pitch, yaw, followTarget.eulerAngles.z);

    }


    private float UpdateRotation(float currentRotation, float input,float min,float max,bool isXAxis)
    {
        currentRotation += isXAxis ? -input : input;
        return Mathf.Clamp(currentRotation, min, max);
    }


    private float GetMouseInput(string axis)
    {
        return Input.GetAxis(axis) * -rotationSpeed;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 마우스 커서 

        if(Input.GetKeyDown(KeyCode.H) && mousecheck ==false)          
        {
            //Cursor.lockState = CursorLockMode.Locked;
            //Cursor.visible = false;
            mousecheck = true;
        }
        else if(Input.GetKeyDown(KeyCode.H) && mousecheck == true)
        {
            //Cursor.lockState = CursorLockMode.None;
            //Cursor.visible = true;
            mousecheck = false;
        }
      
    }
}
