using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[CreateAssetMenu(menuName = "Tile/Factory")]
public class FactoryTileInfo : TileInfo
{
	public string factoryType;

	public void OnEnable()
	{
		TileType = TileType.Factory;
		tag = "Factory";
	}

	public void Craft(Recipe recipe, SettlementTile settlement)
	{
		var inA = recipe.inputA;
		var inB = recipe.inputB;
		if (inA.count <= 0 && inB.count <= 0)
			return;

		if(inA.count > 0)
		{
			if (!settlement.HasResource(inA))
				return;
		}
		if (inB.count > 0)
		{
			if (!settlement.HasResource(inB))
				return;
		}
		settlement.TakeResource(inA);
		settlement.TakeResource(inB);
		settlement.AddResource(recipe.output, recipe.outputCount);
	}
}
