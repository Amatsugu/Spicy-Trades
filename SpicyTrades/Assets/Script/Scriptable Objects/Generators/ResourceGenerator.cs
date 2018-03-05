using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Feature Generator/Resources")]
public class ResourceGenerator : FeatureGenerator
{
	public int maxResources = 3;
	public GameObject[] resources;
	public override void Generate(Map map)
	{
		Debug.Log("Resource Generate");
		var towns = map.GetTowns();
		foreach(TownTile t in towns)
		{
			var candicadates = t.GetNeighbors().SelectMany(n => from Tile nt in n.GetNeighbors() where nt.tag == "Ground" select nt).Distinct().ToList();
			var r = Random.Range(1, maxResources + 1);
			for(int i = 0; i < r; i++)
			{
				map.ReplaceTile(candicadates[i], resources[Random.Range(0, resources.Length)], false, true);
				candicadates.Remove(candicadates[i]);
			}
		}
	}
}
