using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CappedFloat
{
	float _lowerCap;
	float _upperCap;
	float _value;
	public float Value
	{
		get
		{
			return _value;
		}
		set
		{
			_value = Mathf.Clamp(value, _lowerCap, _upperCap);
		}
	}

	public CappedFloat(float lowerCap, float upperCap)
	{
		_lowerCap = lowerCap;
		_upperCap = upperCap;
	}
}
