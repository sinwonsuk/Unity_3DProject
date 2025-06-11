using System.Collections;
using UnityEngine;
using Cinemachine;

public class LobbyCameraManager : MonoBehaviour
{
    public static LobbyCameraManager Instance { get; private set; }

    public CinemachineVirtualCamera virtualCamera;
    public float zoomSpeed = 2f;

    [SerializeField] private Vector3 defaultCamPosition = new Vector3(12.33f, 2.47f, 1.47f);
    [SerializeField] private Vector3 defaultCamRotationEuler = new Vector3(0f, -110.948f, 0f);

    private Vector3 originalCamPos;
    private Quaternion originalCamRot;

    private bool isZoomedIn = false;
    public bool IsZoomedIn => isZoomedIn; // 외부 접근

    private void Awake()
    {
        Instance = this;
        
        originalCamPos = defaultCamPosition;
        originalCamRot = Quaternion.Euler(defaultCamRotationEuler);
    }

    private void Start()
    {
    }

    public void ZoomToCharacter(Transform target)
    {
        if (isZoomedIn) return;
        isZoomedIn = true;

        StopAllCoroutines(); // 기존 코루틴이 돌고 있다면 정지
        StartCoroutine(ZoomCoroutine(target));
    }

    private IEnumerator ZoomCoroutine(Transform target)
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
        StartCoroutine(ZoomBackCoroutine());
    }

    private IEnumerator ZoomBackCoroutine()
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
    }
}
