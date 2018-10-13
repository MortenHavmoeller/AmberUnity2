using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInteraction : MonoBehaviour
{
	Vector2 lastMousePos;

	private void Awake()
	{
		lastMousePos = Input.mousePosition;
	}

	private void OnGUI()
	{
		Event evt = Event.current;
		
		if (evt.isMouse)
		{
			Vector2 mouseDelta = evt.mousePosition - lastMousePos;
			

		}
	}

	// update gets called after any pending OnGUI calls
	private void Update()
	{
		lastMousePos = Input.mousePosition;
	}
}
