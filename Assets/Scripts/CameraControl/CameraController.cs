using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	private const float OFFSET_X = 1f;
	private const float OFFSET_Y = -5f;

	private const int DEFAULT_HEIGHT_STEP = 5;
	private const int MINIMUM_ZOOM_STEP = 3;
	private const int MAXIMUM_ZOOM_STEP = 16;

	private float minimumHeight;
	private float maximumHeight;
	private float cameraHeight;

	private const float HEIGHT_INFLUENCE_SPEED = 6f;
	private const float HEIGHT_INFLUENCE_OFFSET = 2f;

	public AnimationCurve response;
	public float speed = 5f;

	private Vector2 aimPointOffset = new Vector2(OFFSET_X, OFFSET_Y);

	//private Vector2 movement = new Vector2();
	private int zoomStep = DEFAULT_HEIGHT_STEP;

	private Vector3 aimPositionMovement = new Vector3();
	private Vector3 aimPosition = new Vector3();

	private void Start()
	{
		// calculate som standard values
		zoomStep = MINIMUM_ZOOM_STEP;
		minimumHeight = GetHeight();
		zoomStep = MAXIMUM_ZOOM_STEP;
		maximumHeight = GetHeight();

		// set up
		zoomStep = DEFAULT_HEIGHT_STEP;
		cameraHeight = GetHeight();
		PlaceCamera();
	}

	private void Update()
	{
		zoomStep = Mathf.Clamp(zoomStep - (int)(Input.GetAxis("Scroll")), MINIMUM_ZOOM_STEP, MAXIMUM_ZOOM_STEP + 1);

		aimPositionMovement[0] = Input.GetAxis("Horizontal");
		aimPositionMovement[1] = 0;
		aimPositionMovement[2] = Input.GetAxis("Vertical");

		if (aimPositionMovement.sqrMagnitude > 1f)
		{
			aimPositionMovement.Normalize();
		}

		aimPositionMovement = Quaternion.LookRotation((aimPosition - transform.position).GetHorizontal(), Vector3.up) * aimPositionMovement; // bring into camera rotation space

		cameraHeight = GetHeight();

		float speedNow = GetSpeedUsingHeight();

		aimPosition[0] += aimPositionMovement[0] * Time.deltaTime * speedNow;
		aimPosition[1] = 0;
		aimPosition[2] += aimPositionMovement[2] * Time.deltaTime * speedNow;

		PlaceCamera();
	}

	private void PlaceCamera()
	{
		float additionalOffset = Mathf.InverseLerp(MINIMUM_ZOOM_STEP, MAXIMUM_ZOOM_STEP + 1, zoomStep);

		additionalOffset = 1 + additionalOffset * HEIGHT_INFLUENCE_OFFSET;
		transform.position = aimPosition + new Vector3(aimPointOffset.x * additionalOffset, cameraHeight, aimPointOffset.y * additionalOffset);
		transform.LookAt(aimPosition, Vector3.up);
	}

	private float GetSpeedUsingHeight()
	{
		return speed * (1 + HEIGHT_INFLUENCE_SPEED * Mathf.InverseLerp(minimumHeight, maximumHeight + 1, cameraHeight));
	}

	private float GetHeight()
	{
		return zoomStep + Mathf.Exp((zoomStep - MINIMUM_ZOOM_STEP) * 0.25f) - 1;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(aimPosition, 0.25f);
	}
}
