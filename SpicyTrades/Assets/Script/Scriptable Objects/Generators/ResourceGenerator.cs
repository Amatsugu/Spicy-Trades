using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Feature Generator/Resources")]
public class ResourceGenerator : FeatureGenerator
{
	public int maxResources = 3;
	public GameObject resourceTile;
	public ResourceListProvider resourceProvider;
	public override void Generate(Map map)
	{
		GeneratorName = "<b>" + this.GetType().ToString() + ":</b> ";
		var towns = map.GetTowns();
		var resources = resourceProvider.GetResourceList();
		foreach(TownTile t in towns)
		{
			var curTown = (t.tileInfo as TownTileInfo);
			if (curTown.townType == TownType.Town)
			{
				var candicadates = t.GetNeighbors().SelectMany(n => from Tile nt in n.GetNeighbors() where nt != null && nt.Tag == "Ground" select nt).Distinct().ToList();
				var numResources = Random.Range(1, maxResources + 1);
				var placedResources = new List<ResourceTileInfo>();
				//Place Resources
				for (int i = 0; i < numResources; i++)
				{
					int c = Random.Range(0, candicadates.Count);
					var res = map.ReplaceTile(candicadates[c], resources[Random.Range(0, resources.Length)], false, true).tileInfo as ResourceTileInfo;
					placedResources.Add(res);
					t.AddResource(res);
					t.Population += res.requiredWorkers;
					candicadates.RemoveAt(c);
				}
				//Allocate Food
				float foodPerPop = .5f;
				float requiredFood = t.Population * foodPerPop;
				float foodTotal = placedResources.Sum(res => res.category == ResourceCategory.Food ? res.yeild : 0);
				if(requiredFood > foodTotal)
				{
					float neededFood = requiredFood - foodTotal;
					int numFoodTiles = Mathf.CeilToInt(neededFood / resourceProvider.basicFood.yeild);
					Debug.Log(GeneratorName + "Need " + neededFood + " [" + numFoodTiles +"] Food Units");
					for (int i = 0; i < numFoodTiles; i++)
					{
						if (candicadates.Count == 0)
						{
							Debug.Log(GeneratorName + "Needs more food.");
							break;
						}
						int c = Random.Range(0, candicadates.Count); 
						var f = map.ReplaceTile(candicadates[c], resourceProvider.basicFood, false, true).tileInfo as ResourceTileInfo;
						t.Population += resourceProvider.basicFood.requiredWorkers;
						t.AddResource(f);
						candicadates.RemoveAt(c);
					}
				}

				
			}else if(curTown.townType == TownType.Village)
			{
				var candicadates = t.GetNeighbors().SelectMany(n => from Tile nt in n.GetNeighbors() where nt != null && nt.Tag == "Ground" select nt).Distinct().ToList();
				var numResources = Random.Range(1, maxResources + 1);
				var placedResources = new List<ResourceTileInfo>();
				var foods = (from ResourceTileInfo res in resources where res.category == ResourceCategory.Food select res).ToArray();
				//Place Resources
				for (int i = 0; i < numResources; i++)
				{
					int c = Random.Range(0, candicadates.Count);
					var res = map.ReplaceTile(candicadates[c], foods[Random.Range(0, foods.Length)], false, true).tileInfo as ResourceTileInfo;
					placedResources.Add(res);
					t.AddResource(res);
					t.Population += res.requiredWorkers;
					candicadates.RemoveAt(c);
				}
			}
		}
	}
}
