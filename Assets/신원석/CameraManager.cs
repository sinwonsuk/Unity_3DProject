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

    // �Ÿ��� �ε巴�� ����
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
        // �ڷ�ƾ ���� �� ī�޶� üũ �ߴ�
        isCameraCheck = false;

        // FramingTransposer �� ���� ��������
        var ft = camera.GetCinemachineComponent<CinemachineFramingTransposer>();

        // m_CameraDistance�� 1f �̻��� ���� �ε巴�� ��ܿ�
        while (ft.m_CameraDistance > 1f)
        {
            ft.m_CameraDistance = Mathf.MoveTowards( ft.m_CameraDistance,1f,Time.deltaTime * zoomSpeed);
            yield return null;
        }

        // ��ǥġ�� �������� ���� ī�޶� üũ �簳
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
