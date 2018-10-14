using System;

namespace Amber.Interaction.TileMapCommands
{
	public class TileTypeChangeCommand : ICommand
	{
		private IndoorTileMap _map;
		private UInt32 _tileID;

		private UInt16 _newType;
		private UInt16 _oldType;

		public TileTypeChangeCommand(UInt32 tileID, UInt16 newType, IndoorTileMap map)
		{
			_map = map;
			_tileID = tileID;
			_newType = newType;
			_oldType = _map.GetTileDataCopy(tileID).tileType;
		}

		public void Execute()
		{
			IndoorTile tile = _map.GetTile(_tileID);
			tile.ChangeTileType(_newType);
		}

		public void Undo()
		{
			IndoorTile tile = _map.GetTile(_tileID);
			tile.ChangeTileType(_oldType);
		}
	}

}
