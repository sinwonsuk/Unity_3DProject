using Cinemachine;
using System.Collections;
using UnityEngine;

public class CameraManager
{
    CinemachineVirtualCamera camera;
    public bool isCameraCheck = true; 

    public float FirstCameraDistance { get; set; } = 2.0f;

    public CameraManager(CinemachineVirtualCamera camera)
    {
        this.camera = camera;
        
    }

    // 거리를 부드럽게 변경
    public IEnumerator ZoomDistance(float targetDist)
    {
        var ft = camera.GetCinemachineComponent<CinemachineFramingTransposer>();
        if (ft == null) yield break;

        while (Mathf.Abs(ft.m_CameraDistance - targetDist) > 0.01f)
        {
            
            ft.m_CameraDistance = Mathf.MoveTowards(
                ft.m_CameraDistance,
                targetDist,
                Time.deltaTime * zoomSpeed
            ); 
            yield return null;
        }

        isCameraCheck = true;
        ft.m_CameraDistance = targetDist;

    }

    public IEnumerator StartCameraScaleUp()
    {
        // 코루틴 시작 시 카메라 체크 중단
        isCameraCheck = false;

        // FramingTransposer 한 번만 가져오기
        var ft = camera.GetCinemachineComponent<CinemachineFramingTransposer>();

        // m_CameraDistance가 1f 이상일 동안 부드럽게 당겨옴
        while (ft.m_CameraDistance > 1f)
        {
            ft.m_CameraDistance = Mathf.MoveTowards( ft.m_CameraDistance,1f,Time.deltaTime * zoomSpeed);
            yield return null;
        }

        // 목표치에 도달했을 때만 카메라 체크 재개
        isCameraCheck = true;
    }
    public IEnumerator StartCameraScaleDown()
    {
        isCameraCheck = false;
        var ft = camera.GetCinemachineComponent<CinemachineFramingTransposer>();
        while (ft.m_CameraDistance < 2f)
        {
            ft.m_CameraDistance = Mathf.MoveTowards(ft.m_CameraDistance, 2f, Time.deltaTime * zoomSpeed);
            yield return null;
        }
        isCameraCheck = true;
    }
    float zoomSpeed = 1.2f;

}
