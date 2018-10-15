using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Amber.GameWorld.TileMap
{
	[Serializable]
	public struct IndoorTileData
	{
		public UInt32 tileID;
		public UInt16 tileType;
	}

	public class IndoorTile : MonoBehaviour
	{
		private IndoorTileMap _map;
		private UInt32 _tileID;

		private GameObject _model;

		public UInt32 GetID()
		{
			return _tileID;
		}

		public IndoorTileData GetDataCopy()
		{
			return _map.GetTileDataCopy(_tileID);
		}

		public void SetData(IndoorTileData data)
		{
			_map.SetTileData(_tileID, data);
		}

		public static IndoorTile Create(UInt32 tileID, IndoorTileMap map)
		{
			GameObject go = new GameObject(tileID.ToString());
			IndoorTile result = go.AddComponent<IndoorTile>();
			result._map = map;
			result._tileID = tileID;

			result.InstantiateModel();
			return result;
		}

		public void ChangeTileType(UInt16 newType)
		{
			IndoorTileData data = _map.GetTileDataCopy(_tileID);
			data.tileType = newType;
			_map.SetTileData(_tileID, data);
			Cleanup();
			InstantiateModel();
		}

		private void Cleanup()
		{
			if (_model != null)
			{
				_model.transform.parent = null;
				Destroy(_model);
			}
		}

		private void InstantiateModel()
		{
			IndoorTileData data = _map.GetTileDataCopy(_tileID);
			_model = _map.tilePrefabLib.CreateNewInstance(data.tileType);
			_model.transform.parent = transform;
			_model.transform.localPosition = Vector3.zero;
		}
	}
}

