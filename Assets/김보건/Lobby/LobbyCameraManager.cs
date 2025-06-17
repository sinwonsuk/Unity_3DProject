using System.Collections;
using UnityEngine;
using Cinemachine;

public class LobbyCameraManager : MonoBehaviour
{

    private void Awake()
    {
        Instance = this;

    }

    private void Start()
    {
        // 시네머신의 Follow / LookAt 초기화
        virtualCamera.Follow = CameraFocus;
        virtualCamera.LookAt = CameraFocus;

    }

    public void ZoomToCharacter(Transform target)
    {
        if (isZoomedIn) return;
        isZoomedIn = true;

        virtualCamera.Follow = null;
        virtualCamera.LookAt = null;

        //원래 카메라 위치 저장
        originalCamPos = virtualCamera.transform.position;
        originalCamRot = virtualCamera.transform.rotation;

        LobbyUIManager.Instance.chooseCharacterUI.SetActive(false);


        StopAllCoroutines(); // 기존 코루틴이 돌고 있다면 정지
        StartCoroutine(ZoomInCamera(target));
    }

    private IEnumerator ZoomInCamera(Transform target)
    {
        Transform camTransform = virtualCamera.transform;
        Vector3 startPos = camTransform.position;
        Quaternion startRot = camTransform.rotation;

        Vector3 endPos = target.position;
        Vector3 direction = target.position - camTransform.position;
        Quaternion endRot = Quaternion.LookRotation(direction.normalized);

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * zoomSpeed;
            camTransform.position = Vector3.Lerp(startPos, endPos, t);
            camTransform.rotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }
    }

    public void ReturnToOriginalPosition()
    {
        StopAllCoroutines();
        StartCoroutine(ZoomBackCamera()); 

    }

    private IEnumerator ZoomBackCamera()
    {
        Transform camTransform = virtualCamera.transform;
        Vector3 startPos = camTransform.position;
        Quaternion startRot = camTransform.rotation;

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * zoomSpeed;
            camTransform.position = Vector3.Lerp(startPos, originalCamPos, t);
            camTransform.rotation = Quaternion.Slerp(startRot, originalCamRot, t);
            yield return null;
        }

        isZoomedIn = false;
        virtualCamera.Follow = CameraFocus;
        virtualCamera.LookAt = CameraFocus;

        LobbyUIManager.Instance.chooseCharacterUI.SetActive(true);

    }

    public static LobbyCameraManager Instance { get; private set; }

    public CinemachineVirtualCamera virtualCamera;
    public float zoomSpeed = 2f;

    [SerializeField] private Transform CameraFocus;

    private Vector3 originalCamPos;
    private Quaternion originalCamRot;

    private bool isZoomedIn = false;
    public bool IsZoomedIn => isZoomedIn; // 외부 접근
}
