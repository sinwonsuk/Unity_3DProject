using Fusion;
using static Unity.Collections.Unicode;
using UnityEngine;
using Unity.VisualScripting;
using static UnityEngine.UI.Image;
using System.Collections;

public class ShootObj : NetworkBehaviour
{
    // ��Ʈ��ũ ������ ������Ƽ��
    [Networked] public bool flying { get; set; }
    [Networked] public Vector3 flyDir { get; set; }

    Transform originalParent;
    Vector3 originalLocalPosition;
    Quaternion originalLocalRotation;

    //[SerializeField]
    //GameObject material;

    [SerializeField] private float speed = 0.0f;

    public override void FixedUpdateNetwork()
    {

        if (flying)
        {
            transform.position += Runner.DeltaTime * speed * flyDir;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        flying = false;
    }



    public void Shoot(Vector3 dir)
    {
        // ���� ��ġ�� ȸ������ ����

        if ((transform.parent == null))
        {
            Debug.Log("�θ����");
            return;
        }

        originalParent = transform.parent;




        originalLocalPosition = transform.localPosition;
        originalLocalRotation = transform.localRotation;

        Debug.Log($"�߻� ��ġ: {transform.position}");

        flying = true;   // �� �� ���� flying�� flyDir �� �� ��Ʈ��ũ�� ����

       
        flyDir = (dir - transform.position).normalized;

        transform.SetParent(null, true);
        // ���ÿ��� ��� ���̵���
        transform.forward = flyDir;
        //Destroy(gameObject, 10f);

        if (HasStateAuthority)
            StartCoroutine(ReturnToPoolAfter(10f));

    }

    private IEnumerator ReturnToPoolAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        // �̵� ����
        flying = false;

        // ���� ��ġ�� ����
        transform.SetParent(originalParent);
        transform.localPosition = originalLocalPosition;
        transform.localRotation = originalLocalRotation;
        gameObject.SetActive(false);
    }
}