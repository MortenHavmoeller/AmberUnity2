using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Amber.Interaction
{
	public class CommandQueue : MonoBehaviour
	{
		private static CommandQueue _singleton;

		private const int MAX_SAVED_COMMANDS = 64;

		private List<ICommand> _cmdList;

		private void Awake()
		{
			if(_singleton != null)
			{
				Debug.LogError("more than one CommandQueue present");
			}

			_singleton = this;
			_cmdList = new List<ICommand>(MAX_SAVED_COMMANDS);
		}

		public static void Do(ICommand cmd)
		{
			_singleton._Do(cmd);
		}

		private void _Do(ICommand cmd)
		{
			TrimCommandList(1);

			try
			{
				cmd.Execute();
				_cmdList.Add(cmd);
			}
			catch (Exception ex)
			{
				Debug.LogError("command execution exception\n" + ex);

				try
				{
					cmd.Undo();
				}
				catch (Exception ex2)
				{
					Debug.LogError("command undo exception\n" + ex2);
				}
			}
		}

		public static void Undo(int steps)
		{
			_singleton._Undo(steps);
		}

		private void _Undo(int steps)
		{
			while (steps-- > 0 && _cmdList.Count > 0)
			{
				_cmdList[_cmdList.Count - 1].Undo();
				_cmdList.RemoveAt(_cmdList.Count - 1);
			}
		}

		public static int GetQueueCount()
		{
			return _singleton._cmdList.Count;
		}

		private void TrimCommandList(int leftOver)
		{
			while (_cmdList.Count + leftOver >= MAX_SAVED_COMMANDS)
			{
				_cmdList.RemoveAt(0);
			}
		}
	}
}
