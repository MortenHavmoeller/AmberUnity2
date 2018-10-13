using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace Amber.FileHandling
{
	public enum FileType
	{
		IndoorTileMap,
		OutdoorMap
	}

	public class FileOps
	{
		public static int Save(object data, string name, FileType type)
		{
			try
			{
				using (FileStream fs = File.Open(GetPath(type) + "/" + name + GetExt(type), FileMode.OpenOrCreate))
				{
					AmberBinaryFormatter bf = new AmberBinaryFormatter();
					bf.Serialize(fs, data);
					fs.Close();
				}
				return 1;
			}
			catch (Exception ex)
			{
				Debug.LogError("unable to save file\n" + ex);
				return -1;
			}
		}

		public static int SaveJson(string jsonData, string name, FileType type)
		{
			try
			{
				using (FileStream fs = File.Open(GetPath(type) + "/" + name + GetExt(type), FileMode.OpenOrCreate))
				{
					byte[] buffer = Encoding.ASCII.GetBytes(jsonData);
					fs.Write(buffer, 0, buffer.Length);
				}
				return 1;
			}
			catch (Exception ex)
			{
				Debug.LogError("unable to save file\n" + ex);
				return -1;
			}
		}

		public static bool DirExists(FileType type)
		{
			return Directory.Exists(GetPath(type));
		}

		public static int CreateDir(FileType type)
		{
			string path = GetPath(type);
			
			try
			{
				if (!Directory.Exists(path))
				{
					Debug.Log("creating directory: " + path);
					Directory.CreateDirectory(path);
					return 1;
				}
				return 0;
			}
			catch (Exception ex)
			{
				Debug.LogError("unable to create directory\n" + ex);
				return -1;
			}
		}

		public static int Load<T>(string name, FileType type, out T obj) where T : new()
		{
			try
			{
				string destination = GetDestination(name, type);
				if (!File.Exists(destination))
				{
					Debug.LogError("file not found at destination: " + destination);
					obj = new T();
					return 0;
				}
				using (FileStream fs = File.Open(destination, FileMode.Open))
				{
					AmberBinaryFormatter bf = new AmberBinaryFormatter();
					obj = (T) bf.Deserialize(fs);
				}
				return 1;
			}
			catch (Exception ex)
			{
				Debug.LogError("unable to load file\n" + ex);
				obj = new T();
				return -1;
			}
		}

		public static int LoadJson(string name, FileType type, out string jsonData)
		{
			try
			{
				string destination = GetDestination(name, type);
				if (!File.Exists(destination))
				{
					Debug.LogError("file not found at destination: " + destination);
					jsonData = "";
					return 0;
				}
				using (FileStream fs = File.Open(destination, FileMode.Open))
				{
					byte[] buffer = new byte[fs.Length];
					int numBytesToRead = (int)fs.Length;
					int numBytesRead = 0;

					while (numBytesToRead > 0)
					{
						// Read may return anything from 0 to numBytesToRead.
						int n = fs.Read(buffer, numBytesRead, numBytesToRead);

						// Break when the end of the file is reached.
						if (n == 0)
							break;

						numBytesRead += n;
						numBytesToRead -= n;

					}

					jsonData = Encoding.ASCII.GetString(buffer);
				}
				return 1;
			}
			catch (Exception ex)
			{
				Debug.LogError("unable to load file\n" + ex);
				jsonData = "";
				return -1;
			}
		}

		public static bool Exists(string name, FileType type)
		{
			return File.Exists(GetDestination(name, type));
		}


		private static string GetDestination(string name, FileType type)
		{
			return Application.persistentDataPath + "/" + GetFolder(type) + "/" + name + GetExt(type);
		}

		private static string GetPath(FileType type)
		{
			return Application.persistentDataPath + "/" + GetFolder(type);
		}

		private static string GetFolder(FileType type)
		{
			switch (type)
			{
				case FileType.IndoorTileMap:
					return "/maps";
				case FileType.OutdoorMap:
					return "/maps";

				default:
					throw new ArgumentException("file type not recognized; unable to resolve path");
			}
		}

		private static string GetExt(FileType type)
		{
			switch (type)
			{
				case FileType.IndoorTileMap:
					return ".map";
				case FileType.OutdoorMap:
					return ".map";

				default:
					throw new ArgumentException("file type not recognized; unable to resolve extension");
			}
		}
	}
}
