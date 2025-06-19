using System.Collections.Generic;
using UnityEngine;

public class Rope_Physics : MonoBehaviour
{
    public LineRenderer lineRenderer;

    public int segmentCount = 15;
    public int constraintLoop = 15;
    public float segmentLength = 0.1f;
    public float ropeWidth = 0.1f;
    public Vector2 gravity = new Vector2(0.0f, -9.8f);

    [Space(10f)]
    public Transform startTransform;
    public Transform endTransform;
    public Transform middleTransform; // 활시위 당기는 부분

    public List<Segment> segments = new List<Segment>();

    // 탄성 관련
    private bool isDragging = false;
    private Vector2 middleOriginalPosition;
    private Vector2 middleVelocity;
    public float springForce = 5f;
    public float damping = 0.9f;

    private void Start()
    {
        Vector2 segmentPos = startTransform.position;
        for (int i = 0; i < segmentCount; i++)
        {
            segments.Add(new Segment(segmentPos));
            segmentPos.y -= segmentLength;
        }

        middleOriginalPosition = (startTransform.position + endTransform.position) * 0.5f;
    }

    private void Update()
    {
        // 마우스로 중간을 드래그 (테스트용)
        if (Input.GetMouseButton(0))
        {
            Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouse.z = 0;
            middleTransform.position = mouse;
            isDragging = true;
        }
        else
        {
            isDragging = false;
        }

        // 중간 원래 위치 갱신 (활 끝이 움직일 경우)
        middleOriginalPosition = (startTransform.position + endTransform.position) * 0.5f;
    }

    private void FixedUpdate()
    {
        UpdateSegments();

        if (!isDragging)
        {
            ApplySpringForceToMiddle();
        }

        for (int i = 0; i < constraintLoop; i++)
        {
            ApplyConstraint();
        }

        DrawRope();
    }

    private void UpdateSegments()
    {
        for (int i = 0; i < segments.Count; i++)
        {
            segments[i].velocity = segments[i].position - segments[i].previousPos;
            segments[i].previousPos = segments[i].position;
            segments[i].position += gravity * Time.fixedDeltaTime * Time.fixedDeltaTime;
            segments[i].position += segments[i].velocity;
        }
    }

    private void ApplyConstraint()
    {
        if (segments.Count == 0)
            return;

        segments[0].position = startTransform.position;
        segments[segments.Count - 1].position = endTransform.position;

        int middleIndex = segments.Count / 2;
        if (middleTransform != null)
        {
            segments[middleIndex].position = middleTransform.position;
        }

        for (int i = 0; i < segments.Count - 1; i++)
        {
            if (i == 0 || i + 1 == segments.Count - 1) continue; // 양끝 고정
            if (i == middleIndex || i + 1 == middleIndex) continue; // 중간 고정

            float distance = (segments[i].position - segments[i + 1].position).magnitude;
            float difference = (segmentLength - distance);
            Vector2 dir = (segments[i + 1].position - segments[i].position).normalized;
            Vector2 movement = dir * difference;

            segments[i].position -= movement * 0.5f;
            segments[i + 1].position += movement * 0.5f;
        }
    }

    private void DrawRope()
    {
        lineRenderer.startWidth = ropeWidth;
        lineRenderer.endWidth = ropeWidth;

        Vector3[] segmentPositions = new Vector3[segments.Count];
        for (int i = 0; i < segments.Count; i++)
        {
            segmentPositions[i] = segments[i].position;
        }

        lineRenderer.positionCount = segmentPositions.Length;
        lineRenderer.SetPositions(segmentPositions);
    }

    private void ApplySpringForceToMiddle()
    {
        if (middleTransform == null) return;

        Vector2 displacement = (Vector2)middleTransform.position - middleOriginalPosition;
        Vector2 restoringForce = -displacement * springForce;

        middleVelocity += restoringForce * Time.fixedDeltaTime;
        middleVelocity *= damping;

        middleTransform.position += (Vector3)(middleVelocity * Time.fixedDeltaTime);
    }

    public class Segment
    {
        public Vector2 previousPos;
        public Vector2 position;
        public Vector2 velocity;

        public Segment(Vector2 _position)
        {
            previousPos = _position;
            position = _position;
            velocity = Vector2.zero;
        }
    }
}