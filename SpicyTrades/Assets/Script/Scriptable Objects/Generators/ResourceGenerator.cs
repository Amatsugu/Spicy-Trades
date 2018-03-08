using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Feature Generator/Resources")]
public class ResourceGenerator : FeatureGenerator
{
	public int maxResources = 3;
	public GameObject[] resources;
	public GameObject basicFood;
	public override void Generate(Map map)
	{
		GeneratorName = "<b>" + this.GetType().ToString() + ":</b> ";
		var towns = map.GetTowns();
		var foodType = basicFood.GetComponent<ResourceTile>().resourceType;
		foreach(TownTile t in towns)
		{
			if(t.townType.GetType() == typeof(Town))
			{
				var candicadates = t.GetNeighbors().SelectMany(n => from Tile nt in n.GetNeighbors() where nt != null && nt.tag == "Ground" select nt).Distinct().ToList();
				var r = Random.Range(1, maxResources + 1);
				var placedResources = new List<ResourceTile>();
				//Place Resources
				for (int i = 0; i < r; i++)
				{
					int c = Random.Range(0, candicadates.Count);
					var res = map.ReplaceTile(candicadates[c], resources[Random.Range(0, resources.Length)], false, true) as ResourceTile;
					placedResources.Add(res);
					t.AddResource(res);
					t.Population += res.resourceType.requiredWorkers;
					candicadates.RemoveAt(c);
				}
				//Allocate Food
				float foodPerPop = .5f;
				float requiredFood = t.Population * foodPerPop;
				float foodTotal = placedResources.Sum(res => res.resourceType.category == ResourceCategory.Food ? res.resourceType.yeild : 0);
				if(requiredFood > foodTotal)
				{
					float neededFood = requiredFood - foodTotal;
					int numFoodTiles = Mathf.CeilToInt(neededFood / foodType.yeild);
					Debug.Log(GeneratorName + "Need " + neededFood + " [" + numFoodTiles +"] Food Units");
					for (int i = 0; i < numFoodTiles; i++)
					{
						if (candicadates.Count == 0)
						{
							Debug.Log(GeneratorName + "Needs more food.");
							break;
						}
						int c = Random.Range(0, candicadates.Count); 
						var f = map.ReplaceTile(candicadates[c], basicFood, false, true) as ResourceTile;
						t.Population += foodType.requiredWorkers;
						t.AddResource(f);
						candicadates.RemoveAt(c);
					}
				}

				
			}
		}
	}
}
