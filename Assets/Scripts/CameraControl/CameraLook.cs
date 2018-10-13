using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLook : MonoBehaviour
{
	public float sensitivity = 25f;

	Vector2 mouseDelta;
	CircularFloat heading;
	CappedFloat tilt;

	private void Start()
	{
		mouseDelta = new Vector2();
		heading = new CircularFloat(360);
		tilt = new CappedFloat(-60, 60);

		heading.Value = 0;
		tilt.Value = 0;
	}

	private void Update()
	{
		mouseDelta.x = Input.GetAxisRaw("Mouse X") * sensitivity * 0.01f;
		mouseDelta.y = Input.GetAxisRaw("Mouse Y") * sensitivity * 0.01f;
		heading.Value += mouseDelta.x;
		tilt.Value += mouseDelta.y;

		transform.rotation = Quaternion.Euler(-tilt.Value, heading.Value, 0);
	}
}
