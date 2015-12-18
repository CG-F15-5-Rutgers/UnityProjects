#if UNITY_EDITOR
using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
public class RebakeAllScenesEditorScript
{
	[MenuItem ("Upgrade helper/Bake All Scenes")]
	public static void Bake()
	{
		List<string> sceneNames = SearchFiles (Application.dataPath, "*.unity");
		foreach (string f in sceneNames)
		{
			EditorApplication.OpenScene(f);
			
			// Rebake navmesh data
			NavMeshBuilder.BuildNavMesh ();
			
			EditorApplication.SaveScene ();
		}
	}
	static List<string> SearchFiles(string dir, string pattern)
	{
		List <string> sceneNames = new List <string>();
		foreach (string f in Directory.GetFiles(dir, pattern, SearchOption.AllDirectories))
		{
			sceneNames.Add (f);
		}
		return sceneNames;
	}
}
#endif