using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TownType : ScriptableObject
{
	public int MinPopulation;
	public int MaxPopulation;

	public virtual TownTile Initialize(TownTile tile)
	{
		tile.Population = Random.Range(MinPopulation, MaxPopulation);
		return tile;
	}
}
