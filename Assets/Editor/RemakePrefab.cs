using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RemakePrefab : MonoBehaviour
{
	[MenuItem("Commands/Remake Prefab")]
	public static void Generate()
	{
		if (Selection.gameObjects == null || Selection.gameObjects.Length <= 0)
			return;
		var go = Selection.gameObjects[0];
		Instantiate(go);
		/*var components = go.GetComponents<Component>();
		foreach(var comp in components)
		{
			Debug.Log(comp.GetType());
		}
		foreach(Transform child in go.transform)
		{
			Debug.Log(child.name);
		}*/
	}
}
