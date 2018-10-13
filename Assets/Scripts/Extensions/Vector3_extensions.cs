using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3_extensions
{
	public static void Clear(this Vector3 v)
	{
		v.x = 0;
		v.y = 0;
		v.z = 0;
	}
	
	public static void Horizontalize(this Vector3 v)
	{
		v.y = 0;
	}

	public static Vector3 GetHorizontal(this Vector3 v)
	{
		return new Vector3(v.x, 0, v.z);
	}
}
