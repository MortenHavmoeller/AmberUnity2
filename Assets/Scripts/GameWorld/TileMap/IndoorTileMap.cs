using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct IndoorTileMapData
{
	public string name;
	public UInt16 width;
	public UInt16 height;

	public int tileSet;
	public Vector3 ambientLight;
	public Vector3 skyColor;

	// array of tile data here?
	public IndoorTileData[] tiles;

	public static IndoorTileData[] CreateRandomTiles(UInt16 width, UInt16 height)
	{
		IndoorTileData[] tilesData = new IndoorTileData[width * height];
		for (UInt32 i = 0; i < tilesData.Length; i++)
		{
			tilesData[i].tileID = i;
			tilesData[i].tileType = (ushort)UnityEngine.Random.Range(0, 2);
		}
		return tilesData;
	}
}

public class IndoorTileMap : MonoBehaviour
{
	public const uint TILE_SIZE = 5;

	public IndoorTileMapData data;
	public IndoorTilePrefabLib tilePrefabLib;

	private GameObject _floor;
	private IndoorTile[] _tiles;

	private bool initialized = false;

	public void HandleMouseInput(Event mouseEvent)
	{

	}

	public void Initialize(IndoorTileMapData mapData, IndoorTilePrefabLib tilePrefabLib)
	{
		this.tilePrefabLib = tilePrefabLib;

		if (initialized)
		{
			Cleanup();
		}
		
		data = mapData;
		_tiles = new IndoorTile[GetMapSize()];

		if (data.tiles.Length != _tiles.Length)
			throw new InvalidOperationException();

		InstantiateTiles(data);

		initialized = true;
	}

	private void InstantiateTiles(IndoorTileMapData mapData)
	{
		IndoorTile newTile;
		UInt16 x, y;
		for (UInt32 i = 0; i < mapData.tiles.Length; i++)
		{
			newTile = IndoorTile.Create(mapData.tiles[i].tileID, this);
			newTile.transform.parent = transform;
			GetTileIndexes(mapData.tiles[i].tileID, out x, out y);

			newTile.transform.localPosition = new Vector3(x + 0.5f, 0, y + 0.5f) * TILE_SIZE;

			_tiles[mapData.tiles[i].tileID] = newTile;
		}
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

	public IndoorTile GetTile(UInt32 tileID)
	{
		return _tiles[tileID];
	}

	public IndoorTileData GetTileDataCopy(UInt32 tileID)
	{
		return data.tiles[tileID];
	}

	public void SetTileData(UInt32 tileID, IndoorTileData tileData)
	{
		data.tiles[tileID] = tileData;
	}

	public UInt32 GetMapSize()
	{
		return (UInt32)data.width * data.height;
	}

	public UInt32 GetTileID(UInt16 x, UInt16 y)
	{
		return (UInt32)x * (data.height) + y;
	}

	public void GetTileIndexes(UInt32 tileID, out UInt16 x, out UInt16 y)
	{
		x = (UInt16)(tileID % data.height);
		y = (UInt16)(tileID / data.height);
	}

	private void InitializeTileArray()
	{
		_tiles = new IndoorTile[data.width * data.height];
	}
}
