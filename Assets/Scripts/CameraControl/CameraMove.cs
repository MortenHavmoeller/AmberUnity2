using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmberInput;

public class CameraMove : MonoBehaviour
{
	public float speed = 5;
	
	private float movementTime = 0;
	private readonly float movementTimeIncrease = 3f;

	private void Update()
	{
		Vector3 movement = Vector3.zero;
		
		movement.z += Input.GetAxisRaw("Vertical");
		movement.y += Input.GetAxisRaw("Elevation");
		movement.x += Input.GetAxisRaw("Horizontal");
		
		float moveMultiplier = movementTime / movementTimeIncrease;
		float currentSpeed = speed * Mathf.Exp(moveMultiplier);

		if (Input.GetKey(KeyCode.LeftShift))
		{
			currentSpeed *= 5;
		}

		movement.Normalize();
		transform.Translate(movement * Time.deltaTime * currentSpeed, Space.Self);

		if (Mathf.Approximately(movement.sqrMagnitude, 0f))
		{
			movementTime = 0;
		}
		else
		{
			movementTime += Time.deltaTime;
		}
	}
}
