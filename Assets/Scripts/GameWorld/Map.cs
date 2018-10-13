using System;
using System.Collections;
using System.Collections.Generic;
using Amber.FileHandling;
using UnityEngine;

public class Map : MonoBehaviour
{
	private IndoorTileMap _tileMap;

	private GameObject _floor;

	private void Start()
	{
		IndoorTileMapData tileMapData = new IndoorTileMapData();
		tileMapData.name = "test1";
		tileMapData.width = 16;
		tileMapData.height = 16;
		tileMapData.tileSet = 0;
		tileMapData.ambientLight = new Vector3(0.2f, 0.2f, 0.2f);
		tileMapData.skyColor = new Vector3(0f, 0f, 0f);

		//tileMapData.tiles = IndoorTileMap.CreateRandomTiles(16, 16);
		tileMapData.tiles = IndoorTileMap.CreateBlockedTiles(16, 16);

		GameObject go = new GameObject("tile_map");
		_tileMap = go.AddComponent<IndoorTileMap>();
		_tileMap.Initialize(tileMapData);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.K))
		{
			Debug.Log("[user pressed key 'K'] Saving tile data...");

			if (!FileOps.DirExists(FileType.IndoorTileMap))
			{
				FileOps.CreateDir(FileType.IndoorTileMap);
			}

			FileOps.Save(_tileMap.data, "test1", FileType.IndoorTileMap);
		}

		if (Input.GetKeyDown(KeyCode.H))
		{
			Debug.Log("[user pressed key 'H'] Loading tile data...");

			IndoorTileMapData loadedMapData;

			if (FileOps.Load("test1", FileType.IndoorTileMap, out loadedMapData) == 1)
			{
				_tileMap.Initialize(loadedMapData);
				Debug.Log("succesfully loaded data");
			}
		}
	}

	
}
