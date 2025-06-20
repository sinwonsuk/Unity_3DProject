using UnityEngine;

public class ModelRotator : MonoBehaviour
{
	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			prevPos = Input.mousePosition;
		}

		if (Input.GetMouseButton(0))
		{
			Vector3 delta = Input.mousePosition - prevPos;

			transform.Rotate(Vector3.up, -delta.x * rotationSpeed, Space.World);

			if (enableVerticalRotation)
			{
				transform.Rotate(Vector3.right, delta.y * rotationSpeed, Space.Self);
			}

			prevPos = Input.mousePosition;
		}
	}

	private Vector3 prevPos;
	public float rotationSpeed = 0.5f;
	public bool enableVerticalRotation = true; 
}
