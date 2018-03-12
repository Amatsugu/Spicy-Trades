﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum PathMethod
{
	ShortestPath,
	BreathFirstSearch,
	NearestNeighbor,
	None
}

[CreateAssetMenu(menuName = "Feature Generator/Towns")]
public class TownGenerator : FeatureGenerator
{


	public GameObject TownTile;
	public GameObject VillageTile;
	public GameObject CapitalTile;
	public GameObject RoadTile;
	public int maxGenerationCycles = 100;
	public int maxTowns = 8;
	public int minTowns = 4;
	public float minDistance = 5f;
	public PathMethod pathMethod;
	public override void Generate(Map map)
	{
		GeneratorName = "<b>" + this.GetType().ToString() + ":</b> ";
		var capitalCandidates = from Tile t in map where t.tag == "Ground" && t.GetNeighbors().Count(nt => nt != null && nt.tag == "Water") == 3 select t;
		var capitalCandicateCenters = capitalCandidates.SelectMany(t => from Tile nt in t.GetNeighbors() where nt != null && nt.tag == "Ground" select nt);
		var centerCandidates = from Tile t in capitalCandicateCenters where t.GetNeighbors().All(nt => nt != null && nt.tag == "Ground") select t;
		var ccA = centerCandidates.ToArray();
		var capital = ccA[Random.Range(0, ccA.Length)];
		//capital.SetColor(Color.red).SetWeight(0).tag = "Capital"; //Spawn Capital
		capital = map.ReplaceTile(capital, CapitalTile);
		foreach (Tile t in capital.GetNeighbors())
			map.ReplaceTile(t, CapitalTile);
		int numTowns = Random.Range(minTowns, maxTowns);
		int curCycles = 0;
		Tile[] towns = new Tile[numTowns];
		Debug.Log(GeneratorName + "Slecting " + numTowns + " towns...");
		while (curCycles++ < maxGenerationCycles)
		{
			if (numTowns <= 0)
				break;
			Tile townCandidate = map[Random.Range(0, map.TileCount)];
			if (townCandidate.tag != "Ground")
				continue;
			if (!townCandidate.GetNeighbors().All(t => t != null && t.tag == "Ground"))
				continue;
			if (townCandidate.DistanceTo(capital) < minDistance)
				continue;
			if (towns.Any(t => t != null && t.DistanceTo(townCandidate) < minDistance))
				continue;
			towns[--numTowns] = map.ReplaceTile(townCandidate, TownTile).SetWeight(0);
		}
		Debug.Log(GeneratorName + "Finished in " + curCycles + " cycles");
		List<Tile> open = new List<Tile>();
		open.AddRange(towns);
		List<Tile> closed = new List<Tile>
		{
			capital
		};
		var paths = new List<Tile[]>();
		int y = (map.Height / 2), x = (map.Width / 2) - (y/2), z = -x - y;
		var center = map[x,y,z].SetColor(Color.black);
		var middle = towns.Aggregate((a, b) => a.DistanceTo(center) < b.DistanceTo(center) ? a : b).SetColor(Color.cyan);
		switch (pathMethod)
		{
			case PathMethod.ShortestPath:
				while (open.Count > 0)
				{
					var prev = closed.Last();
					paths.Clear();
					foreach(Tile t in open)
					{
						paths.Add(Pathfinder.FindPath(prev, t, null));
					}
					var path = paths.Aggregate((a,b) => a.Length < b.Length ? a : b);
					RenderPath(path, map);
					closed.Add(path.Last());
					open.Remove(path.Last());
			
				}
				break;
			//Method 2
			case PathMethod.BreathFirstSearch:
				var cOrder = GetConnectionOrder(capital);
				for (int i = 0; i < cOrder.Count - 1; i++)
				{
					RenderPath(Pathfinder.FindPath(cOrder[i], cOrder[i + 1]), map);
				}
				break;
			//Method 3
			case PathMethod.NearestNeighbor:
				while (open.Count > 0)
				{
					var prev = closed.Last();
					var closest = open.Aggregate((a, b) => a.DistanceTo(prev) < b.DistanceTo(prev) ? a : b);
					var path = Pathfinder.FindPath(prev, closest);
					closed.Add(closest);
					open.Remove(closest);
					RenderPath(path, map);
				}
				break;
		}
		/*int extra = Random.Range(0, minTowns/2);
		Debug.Log(extra);
		for (int i = 0; i < extra; i++)
		{
			var tileIndex = Random.Range(0, closed.Count);
			int min, max = min = 0;
			if (tileIndex >= closed.Count / 2)
			{
				max = (closed.Count / 2) -1;
				min = 0;
			}else
			{
				min = closed.Count / 2;
				max = closed.Count - 1;
			}
			var tileIndex2 = Random.Range(min, max);
			var path = Pathfinder.FindPath(closed[tileIndex], closed[tileIndex2]);

			RenderPath(path, map);

		}*/

	}


	List<Tile> GetConnectionOrder(Tile start)
	{
		var output = new List<Tile>();
		var visited = new List<Tile>();
		Search(start, output, visited, new string[] { "Town", "Village", "Capital" });
		return output;

	}

	void Search(Tile t, List<Tile> output, List<Tile> visited, string[] tags)
	{
		visited.Add(t);
		if (tags.Contains(t.tag))
			output.Add(t);
		foreach(Tile nt in t.GetNeighbors())
		{
			if (nt == null)
				continue;
			if (visited.Contains(nt))
				continue;
			Search(nt, output, visited, tags);
		}

	}
	


	void RenderPath(Tile[] path, Map map)
	{
		for (int i = 1; i < path.Length - 1; i++)
		{
			if (new string[] { "Town", "Village", "Road", "Capital" }.Any(t => t == path[i].tag))
				continue;
			map.ReplaceTile(path[i], RoadTile, true).SetWeight(0);
		}
	}

	
}
