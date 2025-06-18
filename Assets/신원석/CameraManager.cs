using Cinemachine;
using System.Collections;
using UnityEngine;

public class CameraManager
{
    CinemachineVirtualCamera camera;
    public CameraManager(CinemachineVirtualCamera camera)
    {
        this.camera = camera;
    }


    public IEnumerator StartCameraScaleUp()
    {
        var ft = camera.GetCinemachineComponent<CinemachineFramingTransposer>();

        while (true)
        {
            if (ft.m_CameraDistance == 1)
            {
                yield break;
            }

            ft.m_CameraDistance = Mathf.MoveTowards(ft.m_CameraDistance, 1f, Time.deltaTime * zoomSpeed);

            yield return null;
        }
    }
    public IEnumerator StartCameraScaleDown()
    {
        var ft = camera.GetCinemachineComponent<CinemachineFramingTransposer>();

        while (true)
        {
            if (ft.m_CameraDistance == 2)
            {
                yield break;
            }

            ft.m_CameraDistance = Mathf.MoveTowards(ft.m_CameraDistance, 2f, Time.deltaTime * zoomSpeed);

            yield return null;
        }
    }
    float zoomSpeed = 1.2f;

}
