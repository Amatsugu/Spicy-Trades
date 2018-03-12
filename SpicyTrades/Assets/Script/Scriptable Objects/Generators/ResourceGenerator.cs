using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Feature Generator/Resources")]
public class ResourceGenerator : FeatureGenerator
{
	public int maxResources = 3;
	public GameObject resourceTile;
	public ResourceTileInfo[] resources;
	public ResourceTileInfo basicFood;
	public override void Generate(Map map)
	{
		GeneratorName = "<b>" + this.GetType().ToString() + ":</b> ";
		var towns = map.GetTowns();
		foreach(TownTile t in towns)
		{
			if((t.tileInfo as TownTileInfo).townType == TownType.Town)
			{
				var candicadates = t.GetNeighbors().SelectMany(n => from Tile nt in n.GetNeighbors() where nt != null && nt.Tag == "Ground" select nt).Distinct().ToList();
				var r = Random.Range(1, maxResources + 1);
				var placedResources = new List<ResourceTileInfo>();
				//Place Resources
				for (int i = 0; i < r; i++)
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
					int numFoodTiles = Mathf.CeilToInt(neededFood / basicFood.yeild);
					Debug.Log(GeneratorName + "Need " + neededFood + " [" + numFoodTiles +"] Food Units");
					for (int i = 0; i < numFoodTiles; i++)
					{
						if (candicadates.Count == 0)
						{
							Debug.Log(GeneratorName + "Needs more food.");
							break;
						}
						int c = Random.Range(0, candicadates.Count); 
						var f = map.ReplaceTile(candicadates[c], basicFood, false, true).tileInfo as ResourceTileInfo;
						t.Population += basicFood.requiredWorkers;
						t.AddResource(f);
						candicadates.RemoveAt(c);
					}
				}

				
			}
		}
	}
}
