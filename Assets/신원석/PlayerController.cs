//using UnityEngine;

//public class PlayerController : MonoBehaviour
//{
//    // Start is called once before the first execution of Update after the MonoBehaviour is created
//    void Start()
//    {
//        rigidbody = GetComponent<Rigidbody>();
//    }
//    public int speed = 100;
//    // Update is called once per frame
//    void Update()
//    {
//        // ����, ���� �Է°� �ޱ�
//        float h = Input.GetAxis("Horizontal");
//        float v = Input.GetAxis("Vertical");

//        // ��ü �̵��� ���
//        float moveAmount = Mathf.Clamp01(Mathf.Abs(h) + Mathf.Abs(v));

//        // �Է� ���� ����ȭ
//        var moveInput = (new Vector3(h, 0, v)).normalized;

//        // ī�޶� ������ �������� �̵� ���� ���
//        var moveDir = moveInput;

     
//    }

//    Rigidbody rigidbody;
//}
