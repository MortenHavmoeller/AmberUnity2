using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrueTime : MonoBehaviour
{
	private static TrueTime _singleton;

	private float _updateStartTime;
	private float _lastUpdateStartTime;
	private float _fixedUpdateStartTime;
	private float _lastFixedUpdateStartTime;

	public static float time { get { return _singleton._updateStartTime; } }
	public static float deltaTime { get { return _singleton._updateStartTime - _singleton._lastUpdateStartTime; } }
	public static float fixedTime { get { return _singleton._fixedUpdateStartTime; } }
	public static float fixedDeltaTime { get { return _singleton._fixedUpdateStartTime - _singleton._lastFixedUpdateStartTime; } }
	
	private void Awake()
	{
		float realTime = Time.realtimeSinceStartup;
		_updateStartTime = realTime;
		_lastUpdateStartTime = realTime;
		_fixedUpdateStartTime = realTime;
		_lastFixedUpdateStartTime = realTime;
	}

	private void Update()
	{
		_lastUpdateStartTime = _updateStartTime;
		_updateStartTime = Time.realtimeSinceStartup;
	}

	private void FixedUpdate()
	{
		_lastFixedUpdateStartTime = _fixedUpdateStartTime;
		_fixedUpdateStartTime = Time.realtimeSinceStartup;
	}

	
}
