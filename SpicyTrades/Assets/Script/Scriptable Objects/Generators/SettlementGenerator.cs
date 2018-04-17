using System.Collections;
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

[CreateAssetMenu(menuName = "Feature Generator/Settlements")]
public class SettlementGenerator : FeatureGenerator
{
	public SettlementTileInfo TownTile;
	public SettlementTileInfo VillageTile;
	public SettlementTileInfo CapitalTile;
	public NameProvider townNames;
	public NameProvider villageNames;
	public NameProvider capitalNames;
	public TileInfo RoadTile;
	public int maxGenerationCycles = 100;
	public int maxTowns = 8;
	public int minTowns = 4;
	public int maxVillages = 5;
	public int minVillages = 2;
	public float minDistance = 5f;
	public PathMethod pathMethod;
	public override void Generate(Map map)
	{
		//Capital Selection
		var capitalCandidates = map.Where(t => t.Tag == "Ground" && t.GetNeighbors().Count(nt => nt != null && nt.Tag == "Water") == 3);
		var capitalCandicateCenters = capitalCandidates.SelectMany(t => t.GetNeighbors().Where(nt => nt != null && nt.Tag == "Ground"));
		var centerCandidates = capitalCandicateCenters.Where(t => t.GetNeighbors().All(nt => nt != null && nt.Tag == "Ground"));
		var ccA = centerCandidates.ToArray();
		//capital.SetColor(Color.red).SetWeight(0).tag = "Capital"; //Spawn Capital
		var capital = map.MakeCapital(ccA[Random.Range(0, ccA.Length - 1)], CapitalTile);
		capital.Name = capitalNames.GetNameList().GetNextName();
		foreach (var c in capital.GetNeighbors())
			(c as SettlementTile).Center = capital;
		int numTowns = Random.Range(minTowns, maxTowns);
		int curCycles = 0;
		SettlementTile[] towns = new SettlementTile[numTowns];
		//Town Generation
		Debug.Log(GeneratorName + "Slecting " + numTowns + " towns...");
		var townNameProdider = townNames.GetNameList();
		while (curCycles++ < maxGenerationCycles)
		{
			if (numTowns <= 0)
				break;
			Tile townCandidate = map[Random.Range(0, map.TileCount)];
			if (townCandidate.Tag != "Ground")
				continue;
			if (!townCandidate.GetNeighbors().All(t => t != null && t.Tag == "Ground"))
				continue;
			if (townCandidate.DistanceTo(capital) < minDistance)
				continue;
			if (towns.Any(t => t != null && t.DistanceTo(townCandidate) < minDistance))
				continue;
			var town = map.MakeTown(townCandidate, TownTile).SetWeight(0) as SettlementTile;
			town.Name = townNameProdider.GetNextName();
			towns[--numTowns] = town;
		}
		Debug.Log(GeneratorName + "Finished in " + curCycles + " cycles");
		//Village Generator
		int numVillages = Random.Range(minVillages, maxVillages);
		SettlementTile[] villages = new SettlementTile[numVillages];
		curCycles = 0;
		Debug.Log(GeneratorName + "Slecting " + numVillages + " villages...");
		var villageNameProdider = villageNames.GetNameList();
		while (curCycles++ < maxGenerationCycles)
		{
			if (numVillages <= 0)
				break;
			Tile villageCandidate = map[Random.Range(0, map.TileCount)];
			if (villageCandidate.Tag != "Ground")
				continue;
			if (!villageCandidate.GetNeighbors().All(t => t != null && t.Tag == "Ground"))
				continue;
			if (villageCandidate.DistanceTo(capital) < minDistance)
				continue;
			if (towns.Any(t => t != null && t.DistanceTo(villageCandidate) < minDistance) || villages.Any(t => t != null && t.DistanceTo(villageCandidate) < minDistance))
				continue;
			var village = map.MakeTown(villageCandidate, VillageTile).SetWeight(0) as SettlementTile;
			village.Name = villageNameProdider.GetNextName();
			villages[--numVillages] = village;
		}
		Debug.Log(GeneratorName + "Finished in " + curCycles + " cycles");
		//Generating Roads
		Debug.Log(GeneratorName + "Generating Roads");
		List<Tile> open = new List<Tile>();
		open.AddRange(towns);
		open.AddRange(villages);
		List<Tile> closed = new List<Tile>
		{
			capital
		};
		var paths = new List<Tile[]>();
		//int y = (map.Height / 2), x = (map.Width / 2) - (y/2), z = -x - y;
		//var center = map[x,y,z].SetColor(Color.black);
		//var middle = towns.Aggregate((a, b) => a.DistanceTo(center) < b.DistanceTo(center) ? a : b).SetColor(Color.cyan);
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
		Debug.Log(GeneratorName + "Finished");
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
		if (tags.Contains(t.Tag))
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
			if (new string[] { "Town", "Village", "Road", "Capital" }.Any(t => t == path[i].Tag))
				continue;
			map.ReplaceTile(path[i], RoadTile, true).SetWeight(0);
		}
	}

	
}
