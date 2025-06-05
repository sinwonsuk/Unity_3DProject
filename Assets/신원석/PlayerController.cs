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
//        // 수평, 수직 입력값 받기
//        float h = Input.GetAxis("Horizontal");
//        float v = Input.GetAxis("Vertical");

//        // 전체 이동량 계산
//        float moveAmount = Mathf.Clamp01(Mathf.Abs(h) + Mathf.Abs(v));

//        // 입력 방향 정규화
//        var moveInput = (new Vector3(h, 0, v)).normalized;

//        // 카메라 방향을 기준으로 이동 방향 계산
//        var moveDir = moveInput;

     
//    }

//    Rigidbody rigidbody;
//}
