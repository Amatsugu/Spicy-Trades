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
	public RecipeListProvider recipeList;
	public FactoryListProvider factoryList;
	public override void Generate(Map map)
	{
		var towns = map.GetTowns();
		var resources = resourceProvider.GetResourceList().Where(r => r.recipe == null).ToArray();
		foreach(SettlementTile t in towns)
		{
			var curTown = (t.tileInfo as SettlementTileInfo);
			if (curTown.settlementType == SettlementType.Town)
			{
				var candicadates = t.GetNeighbors().SelectMany(n => from Tile nt in n.GetNeighbors() where nt != null && nt.Tag == "Ground" select nt).Distinct().ToList();
				var numResources = Random.Range(1, maxResources + 1);
				var placedResources = new List<ResourceTileInfo>();
				//Place Resources
				for (int i = 0; i < numResources; i++)
				{
					int c = Random.Range(0, candicadates.Count);
					var res = map.ReplaceTile<ResourceTile>(candicadates[c], resources[Random.Range(0, resources.Length)], false, true).tileInfo;
					placedResources.Add(res);
					t.RegisterResource(res);
					t.Population += res.requiredWorkers;
					candicadates.RemoveAt(c);
				}

				//Place Factories
				if (recipeList == null)
					return;
				var recipes = recipeList.items.Where(recipe => placedResources.Any(resA => recipe.inputA.Match(resA) || recipe.inputB.Match(resA))).ToList();

				//placedResources.SelectMany(resA => placedResources.SelectMany(resB => recipeList.GetRecipesByIngredients(resA, resB))).Distinct().ToList();
				numResources = Random.Range(1, maxResources / 2);
				for (int i = 0; i < numResources; i++)
				{
					if (recipes.Count == 0)
						break;
					var c = Random.Range(0, candicadates.Count);
					var r = Random.Range(0, recipes.Count);
					var f = factoryList.GetFactoryByType(recipes[r].factoryType);
					var factory = map.ReplaceTile<FactoryTile>(candicadates[c], f, false, true).tileInfo;
					t.RegisterFactory(factory);
					t.RegisterRecipe(recipes[r]);
					t.Population += 10; //TODO: Tune numbers
					recipes.RemoveAll(recipie => recipie.factoryType == recipes[r].factoryType);
					candicadates.RemoveAt(c);
				}

				//Allocate Food
				float foodPerPop = curTown.foodPerPop;
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
							Debug.LogWarning(GeneratorName + "Needs more food.");
							break;
						}
						int c = Random.Range(0, candicadates.Count); 
						var f = map.ReplaceTile<ResourceTile>(candicadates[c], resourceProvider.basicFood, false, true).tileInfo;
						t.Population += resourceProvider.basicFood.requiredWorkers;
						t.RegisterResource(f);
						candicadates.RemoveAt(c);
					}
				}
				
			}else if(curTown.settlementType == SettlementType.Village)
			{
				var candicadates = t.GetNeighbors().SelectMany(n => from Tile nt in n.GetNeighbors() where nt != null && nt.Tag == "Ground" select nt).Distinct().ToList();
				var numResources = Random.Range(1, maxResources + 1);
				var placedResources = new List<ResourceTileInfo>();
				var foods = (from ResourceTileInfo res in resources where res.category == ResourceCategory.Food select res).ToArray();
				//Place Resources
				for (int i = 0; i < numResources; i++)
				{
					int c = Random.Range(0, candicadates.Count);
					var res = map.ReplaceTile<ResourceTile>(candicadates[c], foods[Random.Range(0, foods.Length)], false, true).tileInfo;
					placedResources.Add(res);
					t.RegisterResource(res);
					t.Population += res.requiredWorkers;
					candicadates.RemoveAt(c);
				}
				t.Population = Mathf.CeilToInt(t.Population * .8f);

				//Place Factories
				if (recipeList == null)
					return;
				var recipes = recipeList.items.Where(recipe => placedResources.Any(resA => recipe.inputA.Match(resA) || recipe.inputB.Match(resA))).ToList();
				numResources = Random.Range(1, maxResources / 2);
				for (int i = 0; i < numResources; i++)
				{
					if (recipes.Count == 0)
						break;
					var c = Random.Range(0, candicadates.Count);
					var r = Random.Range(0, recipes.Count);
					var f = factoryList.GetFactoryByType(recipes[r].factoryType);
					var factory = map.ReplaceTile<FactoryTile>(candicadates[c], f, false, true).tileInfo;
					t.RegisterFactory(factory);
					t.RegisterRecipe(recipes[r]);
					t.Population += 10; //TODO: Tune numbers
					recipes.RemoveAll(recipie => recipie.factoryType == recipes[r].factoryType);
					candicadates.RemoveAt(c);
				}

				//Allocate Food
				float foodPerPop = curTown.foodPerPop;
				float requiredFood = t.Population * foodPerPop;
				float foodTotal = placedResources.Sum(res => res.category == ResourceCategory.Food ? res.yeild : 0);
				if (requiredFood > foodTotal)
				{
					float neededFood = requiredFood - foodTotal;
					int numFoodTiles = Mathf.CeilToInt(neededFood / resourceProvider.basicFood.yeild);
					Debug.Log(GeneratorName + "Need " + neededFood + " [" + numFoodTiles + "] Food Units");
					for (int i = 0; i < numFoodTiles; i++)
					{
						if (candicadates.Count == 0)
						{
							Debug.LogWarning(GeneratorName + "Needs more food.");
							break;
						}
						int c = Random.Range(0, candicadates.Count);
						var f = map.ReplaceTile<ResourceTile>(candicadates[c], resourceProvider.basicFood, false, true).tileInfo;
						t.Population += resourceProvider.basicFood.requiredWorkers;
						t.RegisterResource(f);
						candicadates.RemoveAt(c);
					}
				}
			}
		}
	}
}
