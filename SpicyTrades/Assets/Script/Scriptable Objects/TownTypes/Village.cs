using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Village : TownType
{
	public override void Initialize(TownTile tile)
	{
		tile.Population = Random.Range(MinPopulation, MaxPopulation);

	}
}
