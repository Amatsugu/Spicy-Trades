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
		var settlements = map.GetSettlements();
		var resources = resourceProvider.GetResourceList().Where(r => r.recipe == null).ToArray();
		foreach(SettlementTile settlement in settlements)
		{
			var curTown = (settlement.tileInfo as SettlementTileInfo);
			if (curTown.settlementType == SettlementType.Town)
			{
				var candidates = settlement.GetNeighbors().SelectMany(n => from Tile nt in n.GetNeighbors() where nt != null && nt.Tag == "Ground" select nt).Distinct().ToList();
				var numResources = Random.Range(1, maxResources + 1);
				var placedResources = new List<ResourceTileInfo>();
				//Place Resources
				for (int i = 0; i < numResources; i++)
				{
					int c = Random.Range(0, candidates.Count);
					var res = map.ReplaceTile<ResourceTile>(candidates[c], resources[Random.Range(0, resources.Length)], false, true).tileInfo;
					placedResources.Add(res);
					settlement.RegisterResource(res);
					settlement.Population += res.requiredWorkers;
					candidates.RemoveAt(c);
				}

				//Place Factories
				GenerateFactories(placedResources, candidates, map, settlement);

				//Allocate Food
				
				
			}else if(curTown.settlementType == SettlementType.Village)
			{
				var candidates = settlement.GetNeighbors().SelectMany(n => from Tile nt in n.GetNeighbors() where nt != null && nt.Tag == "Ground" select nt).Distinct().ToList();
				var numResources = Random.Range(1, maxResources + 1);
				var placedResources = new List<ResourceTileInfo>();
				var foods = (from ResourceTileInfo res in resources where res.category == ResourceCategory.Food select res).ToArray();
				//Place Resources
				for (int i = 0; i < numResources; i++)
				{
					int c = Random.Range(0, candidates.Count);
					var res = map.ReplaceTile<ResourceTile>(candidates[c], foods[Random.Range(0, foods.Length)], false, true).tileInfo;
					placedResources.Add(res);
					settlement.RegisterResource(res);
					settlement.Population += res.requiredWorkers;
					candidates.RemoveAt(c);
				}
				settlement.Population = Mathf.CeilToInt(settlement.Population * .8f);

				//Place Factories
				GenerateFactories(placedResources, candidates, map, settlement);

				//Allocate Food
				float foodPerPop = curTown.foodPerPop;
				float requiredFood = settlement.Population * foodPerPop;
				float foodTotal = placedResources.Sum(res => res.category == ResourceCategory.Food ? res.yeild : 0);
				if (requiredFood > foodTotal)
				{
					float neededFood = requiredFood - foodTotal;
					int numFoodTiles = Mathf.CeilToInt(neededFood / resourceProvider.basicFood.yeild);
					Debug.Log(GeneratorName + "Need " + neededFood + " [" + numFoodTiles + "] Food Units");
					for (int i = 0; i < numFoodTiles; i++)
					{
						if (candidates.Count == 0)
						{
							Debug.LogWarning(GeneratorName + "Needs more food.");
							break;
						}
						int c = Random.Range(0, candidates.Count);
						var f = map.ReplaceTile<ResourceTile>(candidates[c], resourceProvider.basicFood, false, true).tileInfo;
						settlement.Population += resourceProvider.basicFood.requiredWorkers;
						settlement.RegisterResource(f);
						candidates.RemoveAt(c);
					}
				}
			}
		}
	}

	private void AllocateFood()
	{
		float foodPerPop = curTown.foodPerPop;
		float requiredFood = settlement.Population * foodPerPop;
		float foodTotal = placedResources.Sum(res => res.category == ResourceCategory.Food ? res.yeild : 0);
		if (requiredFood > foodTotal)
		{
			float neededFood = requiredFood - foodTotal;
			int numFoodTiles = Mathf.CeilToInt(neededFood / resourceProvider.basicFood.yeild);
			Debug.Log(GeneratorName + "Need " + neededFood + " [" + numFoodTiles + "] Food Units");
			for (int i = 0; i < numFoodTiles; i++)
			{
				if (candidates.Count == 0)
				{
					Debug.LogWarning(GeneratorName + "Needs more food.");
					break;
				}
				int c = Random.Range(0, candidates.Count);
				var f = map.ReplaceTile<ResourceTile>(candidates[c], resourceProvider.basicFood, false, true).tileInfo;
				settlement.Population += resourceProvider.basicFood.requiredWorkers;
				settlement.RegisterResource(f);
				candidates.RemoveAt(c);
			}
		}
	}

	//Place Factories
	private void GenerateFactories(List<ResourceTileInfo> placedResources, List<Tile> candicates, Map map, SettlementTile settlement)
	{
		if (recipeList == null)
			return;
		var recipes = recipeList.items.Where(recipe => placedResources.Any(resA => recipe.inputA.Match(resA) || recipe.inputB.Match(resA))).ToList();
		var numResources = Random.Range(1, maxResources / 2);
		for (int i = 0; i < numResources; i++)
		{
			if (recipes.Count == 0)
				break;
			var c = Random.Range(0, candicates.Count);
			var r = Random.Range(0, recipes.Count);
			var f = factoryList.GetFactoryByType(recipes[r].factoryType);
			var factory = map.ReplaceTile<FactoryTile>(candicates[c], f, false, true).tileInfo;
			settlement.RegisterFactory(factory);
			settlement.RegisterRecipe(recipes[r]);
			settlement.Population += 10; //TODO: Tune numbers
			recipes.RemoveAll(recipie => recipie.factoryType == recipes[r].factoryType);
			candicates.RemoveAt(c);
		}
	}
}
