using UnityEditor;

static class IndoorTilePrefabLibCreator
{
	[MenuItem("Assets/Create/IndoorTilePrefabLib")]
	public static void CreateYourScriptableObject()
	{
		ScriptableObjectUtility.CreateAsset<IndoorTilePrefabLib>();
	}

}