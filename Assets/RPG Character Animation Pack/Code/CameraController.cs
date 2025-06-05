//using UnityEngine;

//public class CameraController : MonoBehaviour
//{
//    GameObject cameraTarget;

//    public float rotateSpeed = 70f;
//    public float offsetDistance = 5f;
//    public float offsetHeight = 3f;
//    public float smoothing = 5f;

//    float currentAngle = 0f;
//    bool following = true;
//    Vector3 lastPosition;

//    void Start()
//    {
//        cameraTarget = GameObject.FindGameObjectWithTag("Player");
//        lastPosition = transform.position;
//    }

//    void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.F))
//            following = !following;

//        float rotateInput = 0f;
//        if (Input.GetKey(KeyCode.Q)) rotateInput = -1f;
//        else if (Input.GetKey(KeyCode.E)) rotateInput = 1f;

//        if (following && cameraTarget != null)
//        {
//            // 🔄 Inspector에서 변경한 rotateSpeed 실시간 반영
//            currentAngle += rotateInput * rotateSpeed * Time.deltaTime;

//            // 🔄 offsetDistance / offsetHeight 실시간 반영
//            Quaternion rotation = Quaternion.Euler(0, currentAngle, 0);
//            Vector3 offset = rotation * new Vector3(0, offsetHeight, -offsetDistance);
//            Vector3 targetPosition = cameraTarget.transform.position + offset;

//            // 🔄 smoothing 값 실시간 반영
//            transform.position = Vector3.Lerp(lastPosition, targetPosition, smoothing * Time.deltaTime);
//            transform.LookAt(cameraTarget.transform.position);
//        }
//        else
//        {
//            transform.position = lastPosition;
//        }
//    }

//    void LateUpdate()
//    {
//        lastPosition = transform.position;
//    }
//}