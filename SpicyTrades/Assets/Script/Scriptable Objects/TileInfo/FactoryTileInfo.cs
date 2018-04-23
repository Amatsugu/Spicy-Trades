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
		var inA = recipe.inputA;
		var inB = recipe.inputB;
		if (inA.count <= 0 && inB.count <= 0)
			return;

		if(inA != null)
		{
			if (!settlement.HasResource(inA))
				return;
		}
		if (inB != null)
		{
			if (!settlement.HasResource(inB))
				return;
		}
		if (settlement.TakeResource(inA) && settlement.TakeResource(inB))
			settlement.AddResource(recipe.output, recipe.outputCount);
	}
}
