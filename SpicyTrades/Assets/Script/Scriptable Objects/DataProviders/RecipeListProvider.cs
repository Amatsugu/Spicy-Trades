using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[CreateAssetMenu(menuName = "Data Providers/Recipe List")]
public class RecipeListProvider : ListProvider<Recipe>
{
	public Recipe[] GetRecipesByIngredient(ResourceTileInfo resource)
	{
		return items.Where(recipe => recipe.inputA.Match(resource) || recipe.inputB.Match(resource)).ToArray();
	}

	public Recipe[] GetRecipesByIngredients(ResourceTileInfo resourceA, ResourceTileInfo resourceB)
	{
		return items.Where(recipe => (recipe.inputA.Match(resourceA) && recipe.inputB.Match(resourceB) || (recipe.inputA.Match(resourceB) && recipe.inputB.Match(resourceA)))).ToArray();
	}
}
