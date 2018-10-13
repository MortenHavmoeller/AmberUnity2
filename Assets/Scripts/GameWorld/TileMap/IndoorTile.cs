using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct IndoorTileData
{
	public UInt32 tileID;
	public UInt16 tileType;
}

public class IndoorTile : MonoBehaviour
{
	public IndoorTileData data; // make reference to data held by the map?

	public static IndoorTile Create(IndoorTileData data)
	{
		GameObject go = new GameObject(data.tileID.ToString());
		IndoorTile result = go.AddComponent<IndoorTile>();
		result.data = data;
		result.InstantiateModel();
		return result;
	}

	private void InstantiateModel()
	{
		if (data.tileType == 0)
			return;

		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
		go.transform.localScale = Vector3.one * IndoorTileMap.TILE_SIZE;
		go.transform.parent = transform;
		go.transform.localPosition = Vector3.zero;
	}
}
