using System.Collections;
using UnityEngine;
using Cinemachine;

public class LobbyCameraManager : MonoBehaviour
{
    public static LobbyCameraManager Instance { get; private set; }

    public CinemachineVirtualCamera virtualCamera;
    public float zoomSpeed = 2f;

    [SerializeField] private Transform fallbackFocus;

    private Vector3 originalCamPos;
    private Quaternion originalCamRot;

    private bool isZoomedIn = false;
    public bool IsZoomedIn => isZoomedIn; // �ܺ� ����

    private void Awake()
    {
        Instance = this;

    }

    private void Start()
    {
        // �ó׸ӽ��� Follow / LookAt �ʱ�ȭ
        virtualCamera.Follow = fallbackFocus;
        virtualCamera.LookAt = fallbackFocus;

        // ī�޶� �ǵ��� ��ġ �����
        originalCamPos = fallbackFocus.position;
        originalCamRot = fallbackFocus.rotation;
    }

    public void ZoomToCharacter(Transform target)
    {
        if (isZoomedIn) return;
        isZoomedIn = true;

        StopAllCoroutines(); // ���� �ڷ�ƾ�� ���� �ִٸ� ����
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
