using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Object = UnityEngine.Object;

public class ImportJSON
{
	[Serializable]
	public class JSONOutput
	{
		public SegmentData[] segments;
	}

	[Serializable]
	public class SegmentData
	{
		public string name;
		public Entity[] obstacles;
		public Entity[] decorations;
		public Entity[] boosts;
	}

	[Serializable]
	public class Entity
	{
		public string name;
		public Vector3 position;
		public Quaternion rotation;
		public Vector3 scale;
	}

	[MenuItem("Commands/Import JSON")]
	public static void Import()
	{
		Dictionary<string, GameObject> segments = new Dictionary<string, GameObject>();
		Dictionary<string, GameObject> entities = new Dictionary<string, GameObject>();

		string path = EditorUtility.OpenFilePanel("Import JSON", "", "json");
		if (path.Length == 0)
			return;
		string json = File.ReadAllText(path);
		var input = JsonUtility.FromJson<JSONOutput>(json);
		var segs = Resources.LoadAll<GameObject>("Segments");
		foreach(var seg in segs)
		{
			segments.Add(seg.name, seg);
		}
		var ents = Resources.LoadAll<GameObject>("Prefabs");
		foreach(var ent in ents)
		{
			entities.Add(ent.name, ent);
		}
		foreach(var seg in input.segments)
		{
			var segmentPrefab = segments[seg.name];
			var segment = Object.Instantiate(segmentPrefab);
			if(seg.decorations != null && seg.decorations.Length > 0)
			{
				var decorRoot = new GameObject("Decorations");
				decorRoot.transform.parent = segment.transform;
				decorRoot.transform.localRotation = Quaternion.Euler(-90, 0, 0);
				foreach (var decor in seg.decorations)
				{
					var decorPrefab = entities[decor.name];
					var decoration = Object.Instantiate(decorPrefab);
					decoration.transform.parent = decorRoot.transform;
					decoration.transform.position = decor.position;
					decoration.transform.rotation = decor.rotation;
					decoration.transform.localScale = decor.scale;
				}
			}

			if (seg.obstacles != null && seg.obstacles.Length > 0)
			{
				var obstacleRoot = new GameObject("Obstacles");
				obstacleRoot.transform.parent = segment.transform;
				obstacleRoot.transform.localRotation = Quaternion.Euler(-90, 0, 0);
				foreach (var obs in seg.obstacles)
				{
					var obsPrefab = entities[obs.name];
					var obstacle = Object.Instantiate(obsPrefab);
					obstacle.transform.parent = obstacleRoot.transform;
					obstacle.transform.position = obs.position;
					obstacle.transform.rotation = obs.rotation;
					obstacle.transform.localScale = obs.scale;
				}
			}

			if (seg.boosts != null && seg.boosts.Length > 0)
			{
				var boostRoot = new GameObject("Powerups");
				boostRoot.transform.parent = segment.transform;
				boostRoot.transform.localRotation = Quaternion.Euler(-90, 0, 0);
				foreach (var boost in seg.boosts)
				{
					var boostPrefab = entities[boost.name];
					var powerUp = Object.Instantiate(boostPrefab);
					powerUp.transform.parent = boostRoot.transform;
					powerUp.transform.position = boost.position;
					powerUp.transform.rotation = boost.rotation;
					powerUp.transform.localScale = boost.scale;
				}
			}
		}
	}
}
