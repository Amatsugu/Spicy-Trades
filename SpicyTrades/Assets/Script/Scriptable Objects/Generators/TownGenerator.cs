using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "Feature Generator/Towns")]
public class TownGenerator : FeatureGenerator
{
	public GameObject TownTile;
	public GameObject RoadTile;
	public int maxGenerationCycles = 100;
	public int maxTowns = 8;
	public int minTowns = 4;
	public float minDistance = 5f;
	public override void Generate(Map map)
	{
		GeneratorName = "<b>" + this.GetType().ToString() + ":</b> ";
		Random.InitState(11);
		var capitalCandidates = from Tile t in map where t.tag == "Ground" && t.GetNeighbors().Count(nt => nt != null && nt.tag == "Water") == 3 select t;
		var capitalCandicateCenters = capitalCandidates.SelectMany(t => from Tile nt in t.GetNeighbors() where nt != null && nt.tag == "Ground" select nt);
		var centerCandidates = from Tile t in capitalCandicateCenters where t.GetNeighbors().All(nt => nt != null && nt.tag == "Ground") select t;
		var ccA = centerCandidates.ToArray();
		var capital = ccA[Random.Range(0, ccA.Length)];
		capital.SetColor(Color.red).SetWeight(0).tag = "Capital"; //Spawn Capital
		int numTowns = Random.Range(minTowns, maxTowns);
		int curCycles = 0;
		Tile[] towns = new Tile[numTowns];
		Debug.Log(GeneratorName + "Slecting " + numTowns + " towns...");
		while(curCycles++ < maxGenerationCycles)
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
		while (open.Count > 0)
		{
			var prev = closed.Last();
			var closest = open.Aggregate((a, b) => a.DistanceTo(prev) < b.DistanceTo(prev) ? a : b);
			var path = Pathfinder.FindPath(prev, closest);
			closed.Add(closest);
			open.Remove(closest);
			for (int i = 1; i < path.Length - 1; i++)
			{
				if (path[i].tag == "Town" || path[i].tag == "Road")
					continue;
				map.ReplaceTile(path[i], RoadTile, true).SetWeight(0);
			}
		}


	}

	
}
