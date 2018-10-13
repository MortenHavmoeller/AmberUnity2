using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceVSync : MonoBehaviour
{
	void Start ()
	{
		Debug.Log("forcing vsync and frame rate");
		Application.targetFrameRate = 60;
		QualitySettings.vSyncCount = 0;
	}
}
