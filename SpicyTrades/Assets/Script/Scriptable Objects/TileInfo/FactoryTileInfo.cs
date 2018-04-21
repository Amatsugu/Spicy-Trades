using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class FactoryTileInfo : TileInfo
{
	public string factoryType;

	public void OnEnable()
	{
		TileType = TileType.Factory;
	}

	public void Craft(Recipe recipe, SettlementTile settlement)
	{
		if (recipe.inputA == null && recipe.inputB == null)
			return;
		if (recipe.inputACount <= 0 && recipe.inputBCount <= 0)
			return;

		if(recipe.inputA != null)
		{
			if (!settlement.HasResource(recipe.inputA, recipe.inputACount))
				return;
		}
		if (recipe.inputB != null)
		{
			if (!settlement.HasResource(recipe.inputB, recipe.inputBCount))
				return;
		}
		if (settlement.TakeResource(recipe.inputA, recipe.inputACount) && settlement.TakeResource(recipe.inputB, recipe.inputBCount))
			settlement.AddResource(recipe.output, recipe.outputCount);
	}
}
