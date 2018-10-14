using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class IndoorTilePrefabLib : ScriptableObject
{
	public GameObject type0;
	public GameObject type1;
	public GameObject type2;
	public GameObject type3;

	public GameObject CreateNewInstance(uint type)
	{
		switch (type)
		{
			case 0:
				return Instantiate(type0);
			case 1:
				return Instantiate(type1);
			case 2:
				return Instantiate(type2);
			case 3:
				return Instantiate(type3);
			default:
				throw new ArgumentOutOfRangeException();
		}
	}
}
