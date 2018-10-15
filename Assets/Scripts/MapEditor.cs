using System;
using System.Collections;
using System.Collections.Generic;
using Amber.FileHandling;
using Amber.Interaction;
using Amber.Interaction.TileMapCommands;
using UnityEngine;
using Amber.GameWorld.Tools;
using Amber.GameWorld.TileMap;

public class MapEditor : MonoBehaviour
{
	private const string TEST_MAP_NAME = "test1";

	public IndoorTilePrefabLib tilePrefabLib;

	private IndoorTileMap _tileMap;
	private GameObject _floor;

	private void Start()
	{
		IndoorTileMapData tileMapData = new IndoorTileMapData();
		tileMapData.name = TEST_MAP_NAME;
		tileMapData.width = 16;
		tileMapData.height = 16;
		tileMapData.tileSet = 0;
		tileMapData.ambientLight = new Vector3(0.2f, 0.2f, 0.2f);
		tileMapData.skyColor = new Vector3(0f, 0f, 0f);

		tileMapData.tiles = IndoorTileMapData.CreateRandomTiles(tileMapData.width, tileMapData.height);

		GameObject go = new GameObject("tile_map");
		_tileMap = go.AddComponent<IndoorTileMap>();

		_tileMap.Initialize(tileMapData, tilePrefabLib);

		LineDrawTool.EnsureExistance();
	}

	private void OnGUI()
	{
		Event evt = Event.current;

		SaveAndLoadButtons(evt);
		EditingButtons(evt);

		ClearTiles(evt);
	}

	private void LateUpdate()
	{
		HighlightTiles(Input.mousePosition);
	}

	private void SaveAndLoadButtons(Event evt)
	{
		if (GUI.Button(RectLayout(0, 0, 0.12f, 0.1f), "Save"))
		{
			evt.Use();
			SaveMap(TEST_MAP_NAME);
		}

		if (GUI.Button(RectLayout(0, 0.11f, 0.12f, 0.1f), "Load"))
		{
			evt.Use();
			IndoorTileMapData loadedMapData;
			if (LoadMap(TEST_MAP_NAME, out loadedMapData))
			{
				_tileMap.Initialize(loadedMapData, tilePrefabLib);
			}
			else
			{
				Debug.LogError("could not load map");
			}
		}
	}

	private void EditingButtons(Event evt)
	{
		if (CommandQueue.GetQueueCount() > 0)
		{
			if (GUI.Button(RectLayout(0.88f, 0, 0.12f, 0.1f), "Undo (" + CommandQueue.GetQueueCount() + " left)"))
			{
				evt.Use();
				CommandQueue.Undo(1);
			}
		}
	}

	private void HighlightTiles(Vector2 mousePos)
	{
		Ray mouseRay = Camera.main.ScreenPointToRay(mousePos);

		GameObject underMouse;
		RaycastHit hitInfo;
		if (Physics.Raycast(mouseRay, out hitInfo))
		{
			underMouse = hitInfo.collider.gameObject;

			IndoorTile tile;
			if (TryGetTile(underMouse, out tile))
			{
				LineDrawTool.DrawBox(underMouse.transform.position, Vector3.one * 5.05f, Color.green);
			}
		}
	}

	private void ClearTiles(Event evt)
	{
		Ray mouseRay = Camera.main.ScreenPointToRay(GetCorrectMousePosition(evt));

		GameObject underMouse;
		RaycastHit hitInfo;
		if (Physics.Raycast(mouseRay, out hitInfo))
		{
			underMouse = hitInfo.collider.gameObject;
			
			if (evt.type == EventType.MouseDown && evt.button == 0)
			{
				IndoorTile tile;
				if (TryGetTile(underMouse, out tile))
				{
					evt.Use();
					IndoorTileData tileData = tile.GetDataCopy();
					UInt16 tileType = tileData.tileType;
					if (tileType == 0)
					{
						tileType = 1;
					}
					else
					{
						tileType = 0;
					}
					
					CommandQueue.Do(new ChangeTileType(tile.GetID(), tileType, _tileMap));
				}
			}
		}
	}



	private Rect RectLayout(float x, float y, float width, float height)
	{
		return new Rect(x * Screen.width, y * Screen.height, width * Screen.width, height * Screen.height);
	}

	private Vector3 GetCorrectMousePosition(Event evt)
	{
		return new Vector3(evt.mousePosition.x, Screen.height - evt.mousePosition.y, 0);
	}

	private void SaveMap(string name)
	{
		FileOps.CreateDirIfNecessary(FileType.IndoorTileMap);

		if (FileOps.Save(_tileMap.data, name, FileType.IndoorTileMap) == 1)
		{
			Debug.Log("succesfully saved data");
		}
	}

	private bool LoadMap(string name, out IndoorTileMapData loadedData)
	{
		if (FileOps.Load(name, FileType.IndoorTileMap, out loadedData) == 1)
		{
			Debug.Log("succesfully loaded data");
			return true;
		}
		loadedData = new IndoorTileMapData();
		return false;
	}

	private bool TryGetTile(GameObject go, out IndoorTile tile)
	{
		if (go.transform.parent == null)
		{
			tile = null;
			return false;
		}

		GameObject parent = go.transform.parent.gameObject;

		if (parent == null)
		{
			tile = null;
			return false;
		}

		tile = parent.GetComponent<IndoorTile>();
		if (tile == null)
		{
			return false;
		}

		return true;
	}
}
