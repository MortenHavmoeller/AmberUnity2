using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ApplicationState
{
	Menu,
	Editor,
	Game
}

public class ApplicationManager
{
	private static ApplicationState state;

	public static ApplicationState GetState()
	{
		return state;
	}
}
