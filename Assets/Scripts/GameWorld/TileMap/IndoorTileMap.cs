using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct IndoorTileMapData
{
	public string name;
	public uint width;
	public uint height;

	public int tileSet;
	public Vector3 ambientLight;
	public Vector3 skyColor;

	// array of tile data here?
	public IndoorTileData[] tiles;
}

public class IndoorTileMap : MonoBehaviour
{
	public const uint TILE_SIZE = 5;

	public IndoorTileMapData data;

	private GameObject _floor;
	private IndoorTile[] _tiles;

	private bool initialized = false;

	public void HandleMouseInput(Event mouseEvent)
	{

	}

	public void Initialize(IndoorTileMapData mapData)
	{
		if (initialized)
		{
			Cleanup();
		}
		else
		{
			_floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
			_floor.name = "floor";
			_floor.transform.localScale = new Vector3(0.1f * mapData.width * TILE_SIZE, 1, 0.1f * mapData.height * TILE_SIZE);
			_floor.transform.parent = transform;
			_floor.transform.localPosition = new Vector3(mapData.width * TILE_SIZE * 0.5f, 0, mapData.height * TILE_SIZE * 0.5f);
		}
		
		data = mapData;
		_tiles = new IndoorTile[GetMapSize()];

		if (data.tiles.Length != _tiles.Length)
			throw new InvalidOperationException();

		IndoorTile newTile;
		UInt16 x, y;
		for (UInt32 i = 0; i < data.tiles.Length; i++)
		{
			newTile = IndoorTile.Create(data.tiles[i]);
			newTile.transform.parent = transform;
			GetTileIndexes(data.tiles[i].tileID, out x, out y);
			newTile.transform.localPosition = new Vector3(x + 0.5f, 0, y + 0.5f) * TILE_SIZE;

			_tiles[data.tiles[i].tileID] = newTile;
		}

		initialized = true;
	}

	public static IndoorTileData[] CreateEmptyTiles(UInt32 width, UInt32 height)
	{
		IndoorTileData[] tilesData = new IndoorTileData[width * height];
		for (UInt32 i = 0; i < tilesData.Length; i++)
		{
			tilesData[i].tileID = i;
			tilesData[i].tileType = 0;
		}
		return tilesData;
	}

	public static IndoorTileData[] CreateBlockedTiles(UInt32 width, UInt32 height)
	{
		IndoorTileData[] tilesData = new IndoorTileData[width * height];
		for (UInt32 i = 0; i < tilesData.Length; i++)
		{
			tilesData[i].tileID = i;
			tilesData[i].tileType = 1;
		}
		return tilesData;
	}

	public static IndoorTileData[] CreateRandomTiles(UInt32 width, UInt32 height)
	{
		IndoorTileData[] tilesData = new IndoorTileData[width * height];
		for (UInt32 i = 0; i < tilesData.Length; i++)
		{
			tilesData[i].tileID = i;
			tilesData[i].tileType = (ushort)UnityEngine.Random.Range(0, 2);
		}
		return tilesData;
	}

	private void Cleanup()
	{
		for (UInt32 i = 0; i < _tiles.Length; i++)
		{
			Destroy(_tiles[i].gameObject);
		}
	}

	public IndoorTile GetTile(UInt16 x, UInt16 y)
	{
		return _tiles[GetTileID(x, y)];
	}

	private void InitializeTileArray()
	{
		_tiles = new IndoorTile[data.width * data.height];
	}

	private UInt32 GetMapSize()
	{
		return data.width * data.height;
	}

	private UInt32 GetTileID(UInt16 x, UInt16 y)
	{
		return x * (data.height) + y;
	}

	private void GetTileIndexes(UInt32 tileID, out UInt16 x, out UInt16 y)
	{
		x = (UInt16)(tileID % data.height);
		y = (UInt16)(tileID / data.height);
	}
}
