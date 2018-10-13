using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularFloat
{
	float _cap;
	float _value;
	public float Value {
		get
		{
			return _value;
		}
		set
		{
			_value = (value < 0) ? _cap + value % _cap : value % _cap;
		}
	}

	public CircularFloat(float cap)
	{
		_cap = cap;
	}

	public static float InstantValue(float val, float cap)
	{
		return (val < 0) ? cap + val % cap : val % cap;
	}
}
